#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using GNU.Gettext;

namespace Hexa.Core.Web.UI
{
    public interface IAction
    {
        string Code { get; }
        string Name { get; }
        int SortKey { get; }
        PageMode PageMode { get; }
    }

    public class ViewAction : IAction
    {

        #region IAction Members

        public string Code
        {
            get { return "View"; }
        }

        public string Name
        {
            get { return GettextHelper.t("View"); }
        }

        public int SortKey
        {
            get { return 1; }
        }

        public PageMode PageMode
        {
            get { return PageMode.ModeView; }
        }

        #endregion
    }

    public class AddAction : IAction
    {

        #region IAction Members

        public string Code
        {
            get { return "New"; }
        }

        public string Name
        {
            get { return GettextHelper.t("Add"); }
        }

        public int SortKey
        {
            get { return 2; }
        }

        public PageMode PageMode
        {
            get { return PageMode.ModeNew; }
        }

        #endregion
    }

    public class UpdateAction : IAction
    {

        #region IAction Members

        public string Code
        {
            get { return "Edit"; }
        }

        public string Name
        {
            get { return GettextHelper.t("Update"); }
        }

        public int SortKey
        {
            get { return 3; }
        }

        public PageMode PageMode
        {
            get { return PageMode.ModeEdit; }
        }

        #endregion
    }

    public class DeleteAction : IAction
    {

        #region IAction Members

        public string Code
        {
            get { return "Delete"; }
        }

        public string Name
        {
            get { return GettextHelper.t("Delete"); }
        }

        public int SortKey
        {
            get { return 4; }
        }

        public PageMode PageMode
        {
            get { return PageMode.ModeDelete; }
        }

        #endregion
    }

    public class CopyAction : IAction
    {

        #region IAction Members

        public string Code
        {
            get { return "Copy"; }
        }

        public string Name
        {
            get { return GettextHelper.t("Copy"); }
        }

        public int SortKey
        {
            get { return 5; }
        }

        public PageMode PageMode
        {
            get { return PageMode.ModeCopy; }
        }

        #endregion
    }

}
