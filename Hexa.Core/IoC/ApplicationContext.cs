//----------------------------------------------------------------------------------------------
// <copyright file="ApplicationContext.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using System.Security.Principal;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Core Context singleton class. Contains a reference to a root CoreContainer object.
    /// </summary>
    public static class ApplicationContext
    {
        public static IPrincipal User
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.User;
                }
                else
                {
                    return Thread.CurrentPrincipal;
                }
            }

            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = value;
                }

                Thread.CurrentPrincipal = value;
            }
        }
    }
}