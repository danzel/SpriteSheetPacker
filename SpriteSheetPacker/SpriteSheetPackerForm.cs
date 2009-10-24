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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SpriteSheetPacker
{
	public partial class SpriteSheetPackerForm : Form
	{
		private static readonly Stopwatch stopWatch = new Stopwatch();

		// our default maximum sprite sheet size
		private const int defaultMaximumSheetWidth = 4096;
		private const int defaultMaximumSheetHeight = 4096;

		// our default image padding
		private const int defaultImagePadding = 0;

		// the valid extensions for images
		private static readonly string[] imageExtensions = new[] { "png", "jpg", "bmp", "gif" };

		// a list of the files we'll build into our sprite sheet
		private readonly List<string> files = new List<string>();

		// did our build succeed
		private bool success;

		public SpriteSheetPackerForm()
		{
			InitializeComponent();

			// set our open file dialog filter based on the valid extensions
			imageOpenFileDialog.Filter = "Image Files|";
			foreach (var ext in imageExtensions) 
				imageOpenFileDialog.Filter += string.Format("*.{0};", ext);

			// set the UI values to our defaults
			maxWidthTxtBox.Text = defaultMaximumSheetWidth.ToString();
			maxHeightTxtBox.Text = defaultMaximumSheetHeight.ToString();
			paddingTxtBox.Text = defaultImagePadding.ToString();
		}

		// determines if a file is an image we accept
		private static bool IsImageFile(string file)
		{
			// ToLower for string comparisons
			string fileLower = file.ToLower();

			// see if the file ends with one of our valid extensions
			foreach (var ext in imageExtensions)
				if (fileLower.EndsWith(ext))
					return true;
			return false;
		}

		private void AddFiles(IEnumerable<string> f)
		{
			foreach (var file in f)
			{
				// make sure the file is an image
				if (!IsImageFile(file))
					continue;

				// prevent duplicates
				if (files.Contains(file)) 
					continue;

				// add to both our internal list and the list box
				files.Add(file);
				listBox1.Items.Add(file);
			}
		}

		private void listBox1_DragEnter(object sender, DragEventArgs e)
		{
			// if this drag is not for a file drop, ignore it
			if (!e.Data.GetDataPresent(DataFormats.FileDrop)) 
				return;

			// figure out if all the files being dropped are images
			bool allImages = true;
			foreach (var f in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				if (!IsImageFile(f))
				{
					allImages = false;
					break;
				}
			}

			// if all the files are images, we're going to "copy" them. otherwise, we don't accept them.
			e.Effect = allImages ? DragDropEffects.Copy : DragDropEffects.None;
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
			if (imageSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				// store the image path
				imageFileTxtBox.Text = imageSaveFileDialog.FileName;

				// if no text path is chosen, make a path with the same name as the image but with the txt extension
				if (string.IsNullOrEmpty(textFileTxtBox.Text))
					textFileTxtBox.Text = imageSaveFileDialog.FileName.Remove(imageSaveFileDialog.FileName.Length - 3) + "txt";
			}
		}

		private void browseTextBtn_Click(object sender, EventArgs e)
		{
			// show the file dialog
			if (textSaveFileDialog.ShowDialog() == DialogResult.OK)
			{
				// store the text file path
				textFileTxtBox.Text = textSaveFileDialog.FileName;

				// if no image path is chosen, make a path with the same name as the text file but with the png extension
				if (string.IsNullOrEmpty(imageFileTxtBox.Text))
					imageFileTxtBox.Text = textSaveFileDialog.FileName.Remove(textSaveFileDialog.FileName.Length - 3) + "png";
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
			if (string.IsNullOrEmpty(textFileTxtBox.Text))
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

			// some dictionaries to hold the image sizes and destination rectangles
			Dictionary<string, Size> imageSizes = new Dictionary<string, Size>();
			Dictionary<string, Rectangle> imagePlacement = new Dictionary<string, Rectangle>();

			// get the sizes of all the images
			foreach (var image in files)
			{
				Bitmap bitmap = Bitmap.FromFile(image) as Bitmap;
				if (bitmap == null)
				{
					ShowBuildError("Could not load " + image + ".");
					return false;
				}
				imageSizes.Add(image, bitmap.Size);
			}

			// sort our files by file size so we place large sprites first
			files.Sort(
				(f1, f2) =>
				{
					Size b1 = imageSizes[f1];
					Size b2 = imageSizes[f2];

					int c = -b1.Width.CompareTo(b2.Width);
					if (c != 0)
						return c;

					return -b1.Height.CompareTo(b2.Height);
				});

			// try to pack the images
			if (!PackImageRectangles(imageSizes, ref outputWidth, ref outputHeight, padding, imagePlacement))
				return false;

			// create the output image
			if (!CreateOutputImage(outputWidth, outputHeight, imagePlacement))
				return false;

			// open a stream writer for the text file
			if (!WriteTextFile(imageSizes, imagePlacement))
				return false;

			return true;
		}

		// This method does some trickery type stuff where we perform the TestPackingImages method over and over, 
		// trying to reduce the image size until we have found the smallest possible image we can fit.
		private bool PackImageRectangles(Dictionary<string, Size> imageSizes, ref int outputWidth, ref int outputHeight, int padding, Dictionary<string, Rectangle> imagePlacement)
		{
			// create a dictionary for our test image placements
			Dictionary<string, Rectangle> testImagePlacement = new Dictionary<string, Rectangle>();

			// get the size of our smallest image
			Size smallestImage = imageSizes[files[files.Count - 1]];

			// we need a couple values for testing
			int testWidth = outputWidth;
			int testHeight = outputHeight;

			// just keep looping...
			while (true)
			{
				// make sure our test dictionary is empty
				testImagePlacement.Clear();

				// try to pack the images into our current test size
				if (!TestPackingImages(imageSizes, testWidth, testHeight, padding, testImagePlacement))
				{
					// if that failed...

					// if we have no images in imagePlacement, i.e. we've never succeeded at PackImages,
					// show an error and return false since there is no way to fit the images into our
					// maximum size texture
					if (imagePlacement.Count == 0)
					{
						ShowBuildError("Output image not large enough to fit all images.");
						return false;
					}

					// otherwise return true to use our last good results
					return true;
				}

				// clear the imagePlacement dictionary and add our test results in
				imagePlacement.Clear();
				foreach (var pair in testImagePlacement)
					imagePlacement.Add(pair.Key, pair.Value);
                
				// figure out the smallest bitmap that will hold all the images
				testWidth = testHeight = 0;
				foreach (var pair in imagePlacement)
				{
					testWidth = Math.Max(testWidth, pair.Value.Right);
					testHeight = Math.Max(testHeight, pair.Value.Bottom);
				}

				// subtract the extra padding on the right and bottom
				testWidth -= padding;
				testHeight -= padding;

				// if we require a power of two texture, find the next power of two that can fit this image
				if (powOf2CheckBox.Checked)
				{
					testWidth = FindNextPowerOfTwo(testWidth);
					testHeight = FindNextPowerOfTwo(testHeight);
				}

				// if we require a square texture, set the width and height to the larger of the two
				if (squareCheckBox.Checked)
				{
					int max = Math.Max(testWidth, testHeight);
					testWidth = testHeight = max;
				}

				// if the test results are the same as our last output results, we've reached an optimal size,
				// so we can just be done
				if (testWidth == outputWidth && testHeight == outputHeight)
					return true;

				// save the test results as our last known good results
				outputWidth = testWidth;
				outputHeight = testHeight;

				// subtract the smallest image size out for the next test iteration
				testWidth -= smallestImage.Width;
				testHeight -= smallestImage.Height;
			}
		}

		private bool TestPackingImages(Dictionary<string, Size> imageSizes, int testWidth, int testHeight, int padding, Dictionary<string, Rectangle> imagePlacements)
		{
			// create the rectangle packer
			ArevaloRectanglePacker rectanglePacker = new ArevaloRectanglePacker(testWidth, testHeight);

			foreach (var image in files)
			{
				// get the bitmap for this file
				Size size = imageSizes[image];

				// pack the image
				Point origin;
				if (!rectanglePacker.TryPack(size.Width + padding, size.Height + padding, out origin))
				{
					return false;
				}

				// add the destination rectangle to our dictionary
				imagePlacements.Add(image, new Rectangle(origin.X, origin.Y, size.Width + padding, size.Height + padding));
			}

			return true;
		}

		private bool CreateOutputImage(int outputWidth, int outputHeight, Dictionary<string, Rectangle> imagePlacement)
		{
			try
			{
				Bitmap outputImage = new Bitmap(outputWidth, outputHeight, PixelFormat.Format32bppArgb);

				// draw all the images into the output image
				foreach (var image in files)
				{
					Rectangle location = imagePlacement[image];
					Bitmap bitmap = Bitmap.FromFile(image) as Bitmap;
					if (bitmap == null)
					{
						ShowBuildError("Could not load " + image + ".");
						return false;
					}

					// copy pixels over to avoid antialiasing or any other side effects of drawing
					// the subimages to the output image using Graphics
					for (int x = 0; x < bitmap.Width; x++)
						for (int y = 0; y < bitmap.Height; y++)
							outputImage.SetPixel(location.X + x, location.Y + y, bitmap.GetPixel(x, y));
				}

				// save the image as a PNG
				outputImage.Save(imageFileTxtBox.Text, ImageFormat.Png);

				return true;
			}
			catch
			{
				ShowBuildError("There was an error building the sprite sheet image.");
				return false;
			}
		}

		private bool WriteTextFile(Dictionary<string, Size> imageSizes, Dictionary<string, Rectangle> imagePlacement)
		{
			try
			{
				using (StreamWriter writer = new StreamWriter(textFileTxtBox.Text))
				{
					// copy the files list and sort alphabetically
					List<string> outputFiles = new List<string>(files);
					outputFiles.Sort();

					foreach (var image in outputFiles)
					{
						// get the bitmap and destination
						Size imageSize = imageSizes[image];
						Rectangle destination = imagePlacement[image];

						// write out the destination rectangle for this bitmap
						writer.WriteLine(string.Format("{0} = {1} {2} {3} {4}", Path.GetFileNameWithoutExtension(image), destination.X, destination.Y, imageSize.Width, imageSize.Height));
					}
				}

				return true;
			}
			catch
			{
				ShowBuildError("There was an error building the sprite sheet text file.");
				return false;
			}
		}

		private static void ShowBuildError(string error)
		{
			MessageBox.Show(error, "Build Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		// stolen from http://en.wikipedia.org/wiki/Power_of_two#Algorithm_to_find_the_next-highest_power_of_two
		private static int FindNextPowerOfTwo(int k)
		{
			k--;
			for (int i = 1; i < sizeof(int) * 8; i <<= 1)
				k = k | k >> i;
			return k + 1;
		}
	}
}
