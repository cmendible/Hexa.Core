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

namespace Hexa.Core.Web.UI
{

    /// <summary>
    /// 
    /// </summary>
	public interface IPageWithOkCancelButtons
	{
        /// <summary>
        /// Gets or sets the page mode.
        /// </summary>
        /// <value>The page mode.</value>
		PageMode PageMode {get; set;}

        /// <summary>
        /// Save pressed.
        /// </summary>
		void SavePressed();

        /// <summary>
        /// Back pressed.
        /// </summary>
		void BackPressed();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is detail.
        /// </summary>
        /// <value><c>true</c> if this instance is detail; otherwise, <c>false</c>.</value>
		bool IsDetail {get; set;}
        
        /// <summary>
		/// Gets or sets the ObjectId.
        /// </summary>
		/// <value>The ObjectId.</value>
		int ObjectId {get; set;}
        
        /// <summary>
        /// Gets or sets a value indicating whether [not updateable].
        /// </summary>
        /// <value><c>true</c> if [not updateable]; otherwise, <c>false</c>.</value>
		bool NotUpdateable {get; set;}
       
        void FillObject(object entity);

		object ObjectInSession { get; set; }

		void SaveUrl();
	}

}

