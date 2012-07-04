#region Header

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

#endregion Header

namespace Hexa.Core.Web.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Validation;

    public class BeforeValidatorCreationEventArgs : EventArgs
    {
        #region Constructors

        public BeforeValidatorCreationEventArgs(IValidationInfo validationInfo)
        {
            ValidationInfo = validationInfo;
            Ignore = false;
        }

        #endregion Constructors

        #region Properties

        public bool Ignore
        {
            get;
            set;
        }

        public IValidationInfo ValidationInfo
        {
            get;
            private set;
        }

        #endregion Properties
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
                     MessageId = "Infos")]
    public class RetrieveValidatorInfosEventArgs : EventArgs
    {
        #region Constructors

        [SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public RetrieveValidatorInfosEventArgs(IList<IValidationInfo> validationInfos)
        {
            ValidationInfos = validationInfos;
        }

        #endregion Constructors

        #region Properties

        [SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public IList<IValidationInfo> ValidationInfos
        {
            get;
            private set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Helper class used to validate entities in a web context
    /// </summary>
    public class WebValidationHelper
    {
        #region Fields

        private readonly Control _Control;
        private readonly Dictionary<Type, BaseConverter> _converters = new Dictionary<Type, BaseConverter>();
        private readonly List<string> _UsedControlIdValues = new List<string>();

        private string _ValidationGroup = "";
        private ValidatorDisplay _ValidatorDisplay = ValidatorDisplay.Static;
        private string _ValidatorText = "";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor for WebValidationHelper
        /// </summary>
        public WebValidationHelper(Control control)
        {
            _Control = control;
            EnsureConvertersAreCreated();
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
            ValidatorDisplay = validatorDisplay;
        }

        /// <summary>
        /// Constructor for WebValidationHelper
        /// </summary>
        /// <param name="control">Parent control (eg Page) that contains input fields</param>
        /// <param name="validationGroup">Validation group name for the validators</param>
        /// <param name="validatorDisplay">ValidatorDisplay mode for the validators</param>
        /// <param name="validatorText">Text property for the validators</param>
        public WebValidationHelper(Control control, string validationGroup, ValidatorDisplay validatorDisplay,
            string validatorText)
            : this(control, validationGroup, validatorDisplay)
        {
            _ValidatorText = validatorText;
        }

        #endregion Constructors

        #region Events

        [SuppressMessage("Microsoft.Naming",
                         "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix")]
        public event EventHandler<BeforeValidatorCreationEventArgs> BeforeValidatorCreation;

        [SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public event EventHandler<RetrieveValidatorInfosEventArgs> RetrieveValidatorInfos;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the Validation group name for the validators
        /// </summary>
        public string ValidationGroup
        {
            get
            {
                return _ValidationGroup;
            }
            set
            {
                _ValidationGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets the ValidatorDisplay mode for the validators
        /// </summary>
        public ValidatorDisplay ValidatorDisplay
        {
            get
            {
                return _ValidatorDisplay;
            }
            set
            {
                _ValidatorDisplay = value;
            }
        }

        /// <summary>
        /// Gets or sets the Text property for the validators
        /// </summary>
        public string ValidatorText
        {
            get
            {
                return _ValidatorText;
            }
            set
            {
                _ValidatorText = value;
            }
        }

        private Control Control
        {
            get
            {
                return _Control;
            }
        }

        private Dictionary<Type, BaseConverter> Converters
        {
            get
            {
                return _converters;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds validator to a given control for an entity type
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <returns>List of validators added to the control</returns>
        [SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validators"),
        SuppressMessage("Microsoft.Design",
                         "CA1004:GenericMethodsShouldProvideTypeParameter"),
        SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<BaseValidator> AddValidators<T>()
        {
            return CreateValidators<T>(true);
        }

        [SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validators"),
        SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<BaseValidator> CreateValidators(bool addToControl, Type entityType)
        {
            bool ignoreValidationInfo = false;
            IList<IValidationInfo> validationInfos = new List<IValidationInfo>();

            if (RetrieveValidatorInfos != null)
            {
                RetrieveValidatorInfos(this, new RetrieveValidatorInfosEventArgs(validationInfos));
            }
            else
            {
                Type providerType = typeof(IValidationInfoProvider<>).MakeGenericType(new[] {entityType});

                var provider = ServiceLocator.GetInstance(providerType) as IValidationInfoProvider;

                validationInfos = provider.GetValidationInfo();
            }

            var validators = new List<BaseValidator>();

            foreach (IValidationInfo validationInfo in validationInfos)
            {
                if (BeforeValidatorCreation != null)
                {
                    var args = new BeforeValidatorCreationEventArgs(validationInfo);
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
                    BaseValidator v = CreateValidator(validationInfo, addToControl);

                    if (v != null)
                    {
                        validators.Add(v);
                    }
                }
            }

            return validators;
        }

        private static BaseValidator GetInputFormatValidator(PropertyInfo pi)
        {
            var validator = new ExtendedRegularExpressionValidator();

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

        private BaseValidator CreateValidator(IValidationInfo validationInfo, bool addToControl)
        {
            Control controlToValidate = Control.Page.FindControlRecursive(validationInfo.PropertyInfo.Name);

            if (controlToValidate == null)
            {
                return null;
            }

            BaseValidator v = null;

            if (validationInfo is IValidateTypeValidationInfo)
            {
                v = GetInputFormatValidator(validationInfo.PropertyInfo);
            }
            else
            {
                v = GetValidator(validationInfo, controlToValidate);
            }

            if (v == null)
            {
                return null;
            }

            Control parentControl = controlToValidate.Parent;

            int indexOf = parentControl.Controls.IndexOf(controlToValidate) + 1;

            v.ID = CreateControlId(indexOf.ToString(CultureInfo.InvariantCulture));

            v.EnableViewState = false;
            v.ControlToValidate = controlToValidate.ID;
            v.ErrorMessage = validationInfo.ErrorMessage;
            v.Text = _ValidatorText;
            v.Display = _ValidatorDisplay;
            v.ValidationGroup = _ValidationGroup;

            if (!addToControl)
            {
                return v;
            }

            controlToValidate.Parent.Controls.AddAt(indexOf, v);

            return v;
        }

        private List<BaseValidator> CreateValidators<T>(bool addToControl)
        {
            return CreateValidators(addToControl, typeof(T));
        }

        private void EnsureConvertersAreCreated()
        {
            if (Converters.Count > 0)
            {
                return;
            }

            Converters.Add(typeof(IRequiredValidationInfo), new RequiredConverter());
            Converters.Add(typeof(IRegexValidationInfo), new RegularExpressionConverter());
            Converters.Add(typeof(IEmailValidationInfo), new EmailConverter());
            Converters.Add(typeof(IRangeValidationInfo), new RangeConverter());
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters",
                         MessageId = "controlToValidate")]
        private BaseValidator GetValidator(IValidationInfo validationInfo, Control controlToValidate)
        {
            KeyValuePair<Type, BaseConverter> converter =
                Converters.Where(c => validationInfo.GetType().GetInterface(c.Key.Name) != null).SingleOrDefault();

            if (converter.Value == null)
            {
                return null;
            }

            return converter.Value.Convert(validationInfo);
        }

        #endregion Methods
    }
}