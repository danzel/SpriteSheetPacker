using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SpriteSheetPacker
{
	public partial class SpriteSheetPackerForm : Form
	{
		// our default maximum sprite sheet size
		private const int DefaultMaximumSheetWidth = 4096;
		private const int DefaultMaximumSheetHeight = 4096;

		// our default image padding
		private const int DefaultImagePadding = 0;

		// the valid extensions for images
		private static readonly string[] imageExtensions = new[] { "png", "jpg", "bmp", "gif" };

		// a list of the files we'll build into our sprite sheet
		private readonly List<string> files = new List<string>();

		public SpriteSheetPackerForm()
		{
			InitializeComponent();

			// set our open file dialog filter based on the valid extensions
			imageOpenFileDialog.Filter = "Image Files|";
			foreach (var ext in imageExtensions) 
				imageOpenFileDialog.Filter += string.Format("*.{0};", ext);

			// set the UI values to our defaults
			maxWidthTxtBox.Text = DefaultMaximumSheetWidth.ToString();
			maxHeightTxtBox.Text = DefaultMaximumSheetHeight.ToString();
			paddingTxtBox.Text = DefaultImagePadding.ToString();
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

				// if no image path is chosen, make a path with the same name as the image but with the txt extension
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
			if (string.IsNullOrEmpty(imageSaveFileDialog.FileName))
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
			if (!int.TryParse(maxWidthTxtBox.Text, out padding) || padding < 0)
			{
				ShowBuildError("Image padding is not a valid non-negative integer");
				return;
			}

			// disable all the controls while the build happens
			foreach (Control control in Controls)
				control.Enabled = false;

			// create a thread to build our sprite sheet and start it
			Thread buildThread = new Thread(BuildThread) { IsBackground = true, Priority = ThreadPriority.AboveNormal };
			buildThread.Start();
		}

		private void BuildThreadComplete()
		{
			// re-enable all our controls
			foreach (Control control in Controls)
				control.Enabled = true;
		}

		private void BuildThread()
		{
			// read our values. we check before this thread, so no worries here
			int outputWidth = int.Parse(maxWidthTxtBox.Text);
			int outputHeight = int.Parse(maxHeightTxtBox.Text);
			int padding = int.Parse(paddingTxtBox.Text);

			// some dictionaries to hold the bitmaps and destination rectangles
			Dictionary<string, Bitmap> imageBitmaps = new Dictionary<string, Bitmap>();
			Dictionary<string, Rectangle> imagePlacement = new Dictionary<string, Rectangle>();

			// create the rectangle packer
			ArevaloRectanglePacker rectanglePacker = new ArevaloRectanglePacker(outputWidth, outputHeight);

			foreach (var image in files)
			{
				// load the bitmap for this file
				Bitmap bitmap = Bitmap.FromFile(image) as Bitmap;

				if (bitmap == null)
				{
					ShowBuildError("Could not load " + image + ".");
					return;
				}

				imageBitmaps.Add(image, bitmap);

				// pack the image
				Point origin;
				if (!rectanglePacker.TryPack(bitmap.Width + padding, bitmap.Height + padding, out origin))
				{
					ShowBuildError("Output image not large enough to fit all images.");
					return;
				}

				// add the destination rectangle to our dictionary
				imagePlacement.Add(image, new Rectangle(origin.X, origin.Y, bitmap.Width + padding, bitmap.Height + padding));
			}

			// reset to 0
			outputWidth = outputHeight = 0;

			// figure out the smallest bitmap that will hold all the images
			foreach (var pair in imagePlacement)
			{
				outputWidth = Math.Max(outputWidth, pair.Value.Right);
				outputHeight = Math.Max(outputHeight, pair.Value.Bottom);
			}
			
			// subtract the extra padding on the right and bottom
			outputWidth -= padding;
			outputHeight -= padding;

			// if we require a power of two texture, find the next power of two that can fit this image
			if (powOf2CheckBox.Checked)
			{
				outputWidth = FindNextPowerOfTwo(outputWidth);
				outputHeight = FindNextPowerOfTwo(outputHeight);
			}

			// if we require a square texture, set the width and height to the larger of the two
			if (squareCheckBox.Checked)
			{
				int max = Math.Max(outputWidth, outputHeight);
				outputWidth = outputHeight = max;
			}

			// create the output image
			Bitmap outputImage = new Bitmap(outputWidth, outputHeight, PixelFormat.Format32bppArgb);

			// create a graphics object from the output
			Graphics graphics = Graphics.FromImage(outputImage);

			// draw all the images into the output image
			foreach (var image in files)
				graphics.DrawImage(imageBitmaps[image], imagePlacement[image]);

			// save the image as a PNG
			outputImage.Save(imageFileTxtBox.Text, ImageFormat.Png);
			
			// open a stream writer for the text file
			using (StreamWriter writer = new StreamWriter(textFileTxtBox.Text))
			{
				foreach (var image in files)
				{
					// get the bitmap and destination
					Bitmap bitmap = imageBitmaps[image];
					Rectangle destination = imagePlacement[image];

					// write out the destination rectangle for this bitmap
					writer.WriteLine(string.Format("{0} = {1} {2} {3} {4}", Path.GetFileNameWithoutExtension(image), destination.X, destination.Y, bitmap.Width, bitmap.Height));

				}
			}

			// invoke the complete method to re-enable our controls
			Invoke(new MethodInvoker(BuildThreadComplete));
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
