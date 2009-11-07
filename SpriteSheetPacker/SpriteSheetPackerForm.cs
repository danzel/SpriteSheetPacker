#region MIT License

/*
 * Copyright (c) 2009 Nick Gravelyn (nick@gravelyn.com), Markus Ewald (cygon@nuclex.org)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software 
 * is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 * PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
 * SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
 * 
 */

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using SpriteSheetPacker.Extensibility;

namespace SpriteSheetPacker
{
	internal partial class SpriteSheetPackerForm : Form
	{
		private static readonly Stopwatch stopWatch = new Stopwatch();
		private static readonly ImagePacker imagePacker = new ImagePacker();

		// our default maximum sprite sheet size
		private const int defaultMaximumSheetWidth = 4096;
		private const int defaultMaximumSheetHeight = 4096;

		// our default image padding
		private const int defaultImagePadding = 1;

		// a list of the files we'll build into our sprite sheet
		private readonly List<string> files = new List<string>();

		// a list of available image and map exporters
		[ImportMany(AllowRecomposition = true)]
		public List<IImageExporter> ImageExporters = new List<IImageExporter>();

		[ImportMany(AllowRecomposition = true)]
		public List<IMapExporter> MapExporters = new List<IMapExporter>();

		// the current image and map exporters
		private IImageExporter currentImageExporter;
		private IMapExporter currentMapExporter;

		// did our build succeed
		private bool success;

		public SpriteSheetPackerForm()
		{
			InitializeComponent();

			// set our open file dialog filter based on the valid extensions
			imageOpenFileDialog.Filter = "Image Files|";
			foreach (var ext in MiscHelper.AllowedImageExtensions) 
				imageOpenFileDialog.Filter += string.Format("*.{0};", ext);

			// set the UI values to our defaults
			maxWidthTxtBox.Text = defaultMaximumSheetWidth.ToString();
			maxHeightTxtBox.Text = defaultMaximumSheetHeight.ToString();
			paddingTxtBox.Text = defaultImagePadding.ToString();
		}

		// configures our image save dialog to take into account all loaded image exporters
		public void GenerateImageSaveDialog()
		{
			string filter = "";
			foreach (var exporter in ImageExporters)
				filter += string.Format("{0} Files|*.{1}|", exporter.Extension.ToUpper(), exporter.Extension);
			filter = filter.Remove(filter.Length - 1);

			imageSaveFileDialog.Filter = filter;
		}

		// configures our map save dialog to take into account all loaded map exporters
		public void GenerateMapSaveDialog()
		{
			string filter = "";
			foreach (var exporter in MapExporters)
				filter += string.Format("{0} Files|*.{1}|", exporter.Extension.ToUpper(), exporter.Extension);
			filter = filter.Remove(filter.Length - 1);

			mapSaveFileDialog.Filter = filter;
		}

        private void AddFiles(IEnumerable<string> paths)
		{
			foreach (var path in paths)
			{
				// if the path is a directory, add all files inside the directory
				if (Directory.Exists(path))
				{
					AddFiles(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
					continue;
				}

				// make sure the file is an image
				if (!MiscHelper.IsImageFile(path))
					continue;

				// prevent duplicates
				if (files.Contains(path)) 
					continue;

				// add to both our internal list and the list box
				files.Add(path);
				listBox1.Items.Add(path);
			}
		}

		private void listBox1_DragEnter(object sender, DragEventArgs e)
		{
			// if this drag is not for a file drop, ignore it
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) 
				return;

			// figure out if any the files being dropped are images
			bool imageFound = false;
			foreach (var f in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				// if the path is a directory and it contains images...
				if (Directory.Exists(f) && MiscHelper.DirectoryContainsImages(f))
				{
					imageFound = true;
					break;
				}

				// or if the path itself is an image
				if (MiscHelper.IsImageFile(f))
				{
					imageFound = true;
					break;
				}
			}

			// if an image is being added, we're going to copy them. otherwise, we don't accept them.
			e.Effect = imageFound ? DragDropEffects.Copy : DragDropEffects.None;
		}

		private void listBox1_DragDrop(object sender, DragEventArgs e)
		{
			// add all the files dropped onto the list box
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				AddFiles((string[])e.Data.GetData(DataFormats.FileDrop));
		}

		private void addImageBtn_Click(object sender, EventArgs e)
		{
			// show our file dialog and add all the resulting files
			if (imageOpenFileDialog.ShowDialog() == DialogResult.OK)
				AddFiles(imageOpenFileDialog.FileNames);
		}

		private void removeImageBtn_Click(object sender, EventArgs e)
		{
			// build a list of files to be removed
			List<string> filesToRemove = new List<string>();
			foreach (var i in listBox1.SelectedItems)
				filesToRemove.Add((string)i);

			// remove the files from our internal list
			files.RemoveAll(filesToRemove.Contains);

			// remove the files from our list box
			filesToRemove.ForEach(f => listBox1.Items.Remove(f));
		}

		private void clearBtn_Click(object sender, EventArgs e)
		{
			// clear both lists
			files.Clear();
			listBox1.Items.Clear();
		}

		private void browseImageBtn_Click(object sender, EventArgs e)
		{
			// show the file dialog
			imageSaveFileDialog.FileName = imageFileTxtBox.Text;
			if (imageSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				// store the image path
				imageFileTxtBox.Text = imageSaveFileDialog.FileName;

				// figure out which image exporter to use based on the extension
				foreach (var exporter in ImageExporters)
				{
					if (exporter.Extension.Equals(Path.GetExtension(imageSaveFileDialog.FileName).Substring(1), StringComparison.InvariantCultureIgnoreCase))
					{
						currentImageExporter = exporter;
						break;
					}
				}
				
				// if there is no selected map exporter, default to the first map exporter
				if (currentMapExporter == null)
				{
					currentMapExporter = MapExporters[0];
				}
				
				mapFileTxtBox.Text = imageSaveFileDialog.FileName.Remove(imageSaveFileDialog.FileName.Length - 3) + currentMapExporter.Extension.ToLower();
			}
		}

		private void browseMapBtn_Click(object sender, EventArgs e)
		{
			// show the file dialog
			mapSaveFileDialog.FileName = mapFileTxtBox.Text;
			if (mapSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				// store the file path
				mapFileTxtBox.Text = mapSaveFileDialog.FileName;

				// figure out which map exporter to use based on the extension
				foreach (var exporter in MapExporters)
				{
					if (exporter.Extension.Equals(Path.GetExtension(mapSaveFileDialog.FileName).Substring(1), StringComparison.InvariantCultureIgnoreCase))
					{
						currentMapExporter = exporter;
						break;
					}
				}

				// if there is no selected image exporter, default to the first image exporter
				if (currentImageExporter == null)
				{
					currentImageExporter = ImageExporters[0];
				}

				imageFileTxtBox.Text = mapSaveFileDialog.FileName.Remove(mapSaveFileDialog.FileName.Length - 3) + currentImageExporter.Extension.ToLower();
			}
		}

		private void buildBtn_Click(object sender, EventArgs e)
		{
			// check our parameters
			if (files.Count == 0)
			{
				ShowBuildError("No images to pack into sheet");
				return;
			}
			if (string.IsNullOrEmpty(imageFileTxtBox.Text))
			{
				ShowBuildError("No image filename given.");
				return;
			}
			if (string.IsNullOrEmpty(mapFileTxtBox.Text))
			{
				ShowBuildError("No text filename given.");
				return;
			}
			int outputWidth, outputHeight, padding;
			if (!int.TryParse(maxWidthTxtBox.Text, out outputWidth) || outputWidth < 1)
			{
				ShowBuildError("Maximum width is not a valid integer value greater than 0.");
				return;
			}
			if (!int.TryParse(maxHeightTxtBox.Text, out outputHeight) || outputHeight < 1)
			{
				ShowBuildError("Maximum height is not a valid integer value greater than 0.");
				return;
			}
			if (!int.TryParse(paddingTxtBox.Text, out padding) || padding < 0)
			{
				ShowBuildError("Image padding is not a valid non-negative integer");
				return;
			}

			// disable all the controls while the build happens
			foreach (Control control in Controls)
				control.Enabled = false;

			// create a thread to build our sprite sheet and start it
			Thread buildThread = new Thread(BuildThread) { IsBackground = true };

			stopWatch.Reset();
			stopWatch.Start();

			buildThread.Start();
		}

		private void BuildThreadComplete()
		{
			stopWatch.Stop();
			if (success)
			{
#if DEBUG
				MessageBox.Show("Build completed in " + stopWatch.Elapsed.TotalSeconds + " seconds.");
#else
				MessageBox.Show("Build Complete", "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
			}

			// re-enable all our controls
			foreach (Control control in Controls)
				control.Enabled = true;
		}

		private void BuildThread()
		{
			// build our sprite sheet
			success = BuildSpriteSheet();

			// invoke the complete method to re-enable our controls
			Invoke(new MethodInvoker(BuildThreadComplete));
		}

		private bool BuildSpriteSheet()
		{
			// read our values. we check before this thread, so no worries here
			int outputWidth = int.Parse(maxWidthTxtBox.Text);
			int outputHeight = int.Parse(maxHeightTxtBox.Text);
			int padding = int.Parse(paddingTxtBox.Text);

			// generate our output
			Bitmap outputImage;
			Dictionary<string, Rectangle> outputMap;
			if (!imagePacker.PackImage(files, powOf2CheckBox.Checked, squareCheckBox.Checked, outputWidth, outputHeight, padding, out outputImage, out outputMap))
			{
				ShowBuildError("There was an error making the image sheet.");
				return false;
			}

			// try to save using our exporters
			try
			{
				currentImageExporter.Save(imageFileTxtBox.Text, outputImage);
				currentMapExporter.Save(mapFileTxtBox.Text, outputMap);
			}
			catch (Exception e)
			{
				ShowBuildError("Error exporting files: " + e.Message);
				return false;
			}

			return true;
		}

		private static void ShowBuildError(string error)
		{
			MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
