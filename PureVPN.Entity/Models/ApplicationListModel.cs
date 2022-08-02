using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PureVPN.Entity.Models
{
    public class ApplicationListModel
    {

        public int Id { get; set; }

        public bool IsActive { get; set; }
        

        [JsonProperty("name")]
        public string Name { get; set; }

        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }


        private System.Drawing.Icon _RawIcon;

        [JsonIgnoreAttribute]
        public System.Drawing.Icon RawIcon
        {
            get
            {
                if (_RawIcon == null)
                {
                    try
                    {
                        _RawIcon = System.Drawing.Icon.ExtractAssociatedIcon(path);
                    }
                    catch(Exception ex)
                    {
                        //*
                    }
                }
                return _RawIcon;
            }
            set
            {
                _RawIcon = value;
            }
        }

        private string exeName;

        public string ExeName
        {
            get { return exeName; }
            set { exeName = value; }
        }

        private List<string> supportingexe;
        public List<string> SupportingExe
        {
            get { return supportingexe; }
            set { supportingexe = value; }
        }


        private bool isItemChecked;

        public bool IsItemChecked
        {
            get { return isItemChecked; }
            set
            {
                isItemChecked = value;
            }
        }

        private bool _isAddedByFileBrowser;
        public bool IsAddedByFileBrowser
        {
            get { return _isAddedByFileBrowser; }
            set
            {
                _isAddedByFileBrowser = value;
            }
        }
    }
}
