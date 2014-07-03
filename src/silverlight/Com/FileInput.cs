using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace Moxiecode.Com
{
	public class FileInput : Button
	{
		private List<File> _files = new List<File>();
        private Boolean doUnzip;

		public static string[] dispatches = new string[] 
		{
			"Cancel", 
			"Change",
			"MouseEnter",
			"MouseLeave",
			"MouseDown",
			"MouseUp"
		};

		public event EventHandler Cancel;
		public event EventHandler Change;
		public event MouseButtonEventHandler MouseDown;
		public event MouseButtonEventHandler MouseUp;

		private bool _multiple = true;
		private string _accept;

		private Boolean _disabled = false;
		

		public FileInput()
		{
			this.Opacity = 0;
			this.Cursor = Cursors.Hand;
			this.Margin = new Thickness(4, 4, 4, 4); // increase probablity of detecting mouseout properly
			this.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.VerticalAlignment = VerticalAlignment.Stretch;
		}

        public void init(object accept, object name, object multiple, object do_checking_unzip)
		{
            this._init((string)accept, (string)name, (bool)multiple, (bool)do_checking_unzip);
		}

        private void _init(string accept, string name, bool multiple, bool do_checking_unzip)
		{
			_multiple = multiple;
			_accept = accept;
            doUnzip = do_checking_unzip;

			//this.MouseLeftButtonUp += new MouseButtonEventHandler(OnClick);
			this.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs args)
			{
				MouseDown(sender, null);
			};

			this.Click += delegate(object sender, RoutedEventArgs args)
			{
				if (!this._disabled) {
					this._openDialog();
				}
			};
		}


		public void disable(object state)
		{
			_disabled = Convert.ToBoolean(state);
			this.Cursor = _disabled ? Cursors.Arrow : Cursors.Hand;
		}


		public Dictionary<string, object>[] getFiles()
		{
			List<Dictionary<string, object>> files = new List<Dictionary<string, object>>();
			foreach (File file in _files)
			{
				files.Add(file.ToObject());
				Moxie.compFactory.add(file.uid, file);
			}
			return files.ToArray();
		}


		private void _openDialog()
		{
			OpenFileDialog dialog = new OpenFileDialog();

			_files.Clear();

			try
			{
				dialog.Multiselect = this._multiple;
				dialog.Filter = this._accept;

				if ((bool)dialog.ShowDialog())
				{
					foreach (FileInfo fileInfo in dialog.Files)
					{
						_files.Add(new File(new List<object>{ fileInfo }));

                        //if the file is zip file, then we read its structure
                        //if the doUnzip is true then unzip the file, otherwise not
                        if (doUnzip && _files[_files.Count - 1].name.Contains(".zip"))
                        {
                            _files[_files.Count - 1].fileNamesInZip = unZipToGetStructures(fileInfo);
                        }
                        else
                        {
                            _files[_files.Count - 1].fileNamesInZip = null;
                        }
					}
					Change(this, null);
				}
				else
				{
					Cancel(this, null);
				}
			}
			catch (Exception ex)
			{
				// throw error
			}
		}

        /// <summary>
        /// unZip the zip file and read the structure, then return as a list
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private List<object> unZipToGetStructures(FileInfo fileInfo)
        {
            using (ZipInputStream s = new ZipInputStream(fileInfo.OpenRead()))
            {
                //each file that been zipped is considered as an object of ZipEntry，by using the method GetNextEntry in ZipInputStream
                //traverse the directory one by one。
                ZipEntry theEntry;

                List<object> fileNamesInZip = new List<object>();

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.IsFile)
                    {
                        File fileTemp = new File(theEntry.Name, theEntry.Size);
                        fileTemp.lastModifiedData = theEntry.DateTime;

                        fileNamesInZip.Add(fileTemp.ToObject());
                    }
                }
                return fileNamesInZip;
            }
        }
	}
}
