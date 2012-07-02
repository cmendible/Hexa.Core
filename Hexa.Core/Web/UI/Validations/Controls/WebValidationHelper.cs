#region License

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hexa.Core.Validation;

namespace Hexa.Core.Web.UI.Controls
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
    public class RetrieveValidatorInfosEventArgs : EventArgs
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public IList<IValidationInfo> ValidationInfos
        {
            get;
            private set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public RetrieveValidatorInfosEventArgs(IList<IValidationInfo> validationInfos)
        {
            ValidationInfos = validationInfos;
        }
    }

    public class BeforeValidatorCreationEventArgs : EventArgs
    {
        public IValidationInfo ValidationInfo
        {
            get;
            private set;
        }

        public bool Ignore
        {
            get;
            set;
        }

        public BeforeValidatorCreationEventArgs(IValidationInfo validationInfo)
        {
            ValidationInfo = validationInfo;
            Ignore = false;
        }
    }

    /// <summary>
    /// Helper class used to validate entities in a web context
    /// </summary>
    public class WebValidationHelper
    {
        private string _ValidationGroup = "";
        private ValidatorDisplay _ValidatorDisplay = ValidatorDisplay.Static;
        private string _ValidatorText = "";

        private Control _Control;

        List<string> _UsedControlIdValues = new List<string>();

        private Control Control
        {
            get
                {
                    return _Control;
                }
        }

        Dictionary<Type, BaseConverter> _converters = new Dictionary<Type, BaseConverter>();
        private Dictionary<Type, BaseConverter> Converters
        {
            get
                {
                    return _converters;
                }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public event EventHandler<RetrieveValidatorInfosEventArgs> RetrieveValidatorInfos;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix")]
        public event EventHandler<BeforeValidatorCreationEventArgs> BeforeValidatorCreation;

        /// <summary>
        /// Constructor for WebValidationHelper
        /// </summary>
        public WebValidationHelper(Control control)
        {
            _Control = control;
            EnsureConvertersAreCreated();
        }

        private void EnsureConvertersAreCreated()
        {
            if (Converters.Count > 0) return;

            Converters.Add(typeof(IRequiredValidationInfo), new RequiredConverter());
            Converters.Add(typeof(IRegexValidationInfo), new RegularExpressionConverter());
            Converters.Add(typeof(IEmailValidationInfo), new EmailConverter());
            Converters.Add(typeof(IRangeValidationInfo), new RangeConverter());
        }

        /// <summary>
        /// Constructor for WebValidationHelper
        /// </summary>
        /// <param name="control">Parent control (eg Page) that contains input fields</param>
        /// <param name="validationGroup">Validation group name</param>
        public WebValidationHelper(Control control, string validationGroup)
        : this(control)
        {
            _ValidationGroup = validationGroup;
        }

        /// <summary>
        /// Constructor for WebValidationHelper
        /// </summary>
        /// <param name="control">Parent control (eg Page) that contains input fields</param>
        /// <param name="validationGroup">Validation group name for the validators</param>
        /// <param name="validatorDisplay">ValidatorDisplay mode for the validators</param>
        public WebValidationHelper(Control control, string validationGroup, ValidatorDisplay validatorDisplay)
        : this(control, validationGroup)
        {
            this.ValidatorDisplay = validatorDisplay;
        }

        /// <summary>
        /// Constructor for WebValidationHelper
        /// </summary>
        /// <param name="control">Parent control (eg Page) that contains input fields</param>
        /// <param name="validationGroup">Validation group name for the validators</param>
        /// <param name="validatorDisplay">ValidatorDisplay mode for the validators</param>
        /// <param name="validatorText">Text property for the validators</param>
        public WebValidationHelper(Control control, string validationGroup, ValidatorDisplay validatorDisplay, string validatorText)
        : this(control, validationGroup, validatorDisplay)
        {
            _ValidatorText = validatorText;
        }

        /// <summary>
        /// Gets or sets the Validation group name for the validators
        /// </summary>
        public string ValidationGroup
        {
            get
                {
                    return this._ValidationGroup;
                }
            set
                {
                    this._ValidationGroup = value;
                }
        }

        /// <summary>
        /// Gets or sets the ValidatorDisplay mode for the validators
        /// </summary>
        public ValidatorDisplay ValidatorDisplay
        {
            get
                {
                    return this._ValidatorDisplay;
                }
            set
                {
                    this._ValidatorDisplay = value;
                }
        }

        /// <summary>
        /// Gets or sets the Text property for the validators
        /// </summary>
        public string ValidatorText
        {
            get
                {
                    return this._ValidatorText;
                }
            set
                {
                    this._ValidatorText = value;
                }
        }

        /// <summary>
        /// Adds validator to a given control for an entity type
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>List of validators added to the control</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validators"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<BaseValidator> AddValidators<T>()
        {
            return CreateValidators<T>(true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validators"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<BaseValidator> CreateValidators(bool addToControl, Type entityType)
        {
            bool ignoreValidationInfo = false;
            IList<IValidationInfo> validationInfos = new List<IValidationInfo>();

            if (RetrieveValidatorInfos != null)
                RetrieveValidatorInfos(this, new RetrieveValidatorInfosEventArgs(validationInfos));
            else
                {
                    Type providerType = typeof(IValidationInfoProvider<>).MakeGenericType(new Type[] { entityType });

                    var provider = ServiceLocator.GetInstance(providerType) as IValidationInfoProvider;

                    validationInfos = provider.GetValidationInfo();
                }

            List<BaseValidator> validators = new List<BaseValidator>();

            foreach (IValidationInfo validationInfo in validationInfos)
                {

                    if (BeforeValidatorCreation != null)
                        {
                            BeforeValidatorCreationEventArgs args = new BeforeValidatorCreationEventArgs(validationInfo);
                            BeforeValidatorCreation(this, args);
                            ignoreValidationInfo = args.Ignore;
                        }
                    else
                        {
                            ignoreValidationInfo = false;
                        }

                    if (!ignoreValidationInfo)
                        {
                            // Extend here with another event.
                            BaseValidator v = this.CreateValidator(validationInfo, addToControl);

                            if (v != null)
                                validators.Add(v);
                        }
                }

            return validators;
        }

        private List<BaseValidator> CreateValidators<T>(bool addToControl)
        {
            return this.CreateValidators(addToControl, typeof(T));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "controlToValidate")]
        private BaseValidator GetValidator(IValidationInfo validationInfo, Control controlToValidate)
        {
            var converter = Converters.Where(c => validationInfo.GetType().GetInterface(c.Key.Name) != null).SingleOrDefault();

            if (converter.Value == null)
                return null;

            return converter.Value.Convert(validationInfo);
        }

        private static BaseValidator GetInputFormatValidator(PropertyInfo pi)
        {
            ExtendedRegularExpressionValidator validator = new ExtendedRegularExpressionValidator();

            if (pi.PropertyType == typeof(int))
                {
                    validator.ValidationExpression = @"[0-9]*";
                }
            else if (pi.PropertyType == typeof(decimal))
                {
                    validator.ValidationExpression = @"[0-9]+[\.\,]?[0-9]+";
                }
            else if (pi.PropertyType == typeof(float))
                {
                    validator.ValidationExpression = @"[0-9]*";
                }
            else
                {
                    return null;
                }

            return validator;
        }

        private BaseValidator CreateValidator(IValidationInfo validationInfo, bool addToControl)
        {
            Control controlToValidate = Control.Page.FindControlRecursive(validationInfo.PropertyInfo.Name);

            if (controlToValidate == null)
                return null;

            BaseValidator v = null;

            if (validationInfo is IValidateTypeValidationInfo)
                v = GetInputFormatValidator(validationInfo.PropertyInfo);
            else
                v = this.GetValidator(validationInfo, controlToValidate);

            if (v == null)
                return null;

            Control parentControl = controlToValidate.Parent;

            int indexOf = parentControl.Controls.IndexOf(controlToValidate) + 1;

            v.ID = CreateControlId(indexOf.ToString(System.Globalization.CultureInfo.InvariantCulture));

            v.EnableViewState = false;
            v.ControlToValidate = controlToValidate.ID;
            v.ErrorMessage = validationInfo.ErrorMessage;
            v.Text = this._ValidatorText;
            v.Display = this._ValidatorDisplay;
            v.ValidationGroup = this._ValidationGroup;

            if (!addToControl)
                return v;

            controlToValidate.Parent.Controls.AddAt(indexOf, v);

            return v;
        }

        private string CreateControlId(string proposedID)
        {
            if (!_UsedControlIdValues.Contains(proposedID))
                {
                    _UsedControlIdValues.Add(proposedID);
                    return "v" + proposedID;
                }
            else
                {
                    return CreateControlId(proposedID + "_1");
                }
        }
    }
}
