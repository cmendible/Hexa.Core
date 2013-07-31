//----------------------------------------------------------------------------------------------
// <copyright file="SqlExceptionHandler.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.SqlClient;
    using System.Text.RegularExpressions;

    using NHibernate.Exceptions;

    public class SqlExceptionHandler : ISQLExceptionConverter
    {
        public Exception Convert(AdoExceptionContextInfo exInfo)
        {
            var sqle = ADOExceptionHelper.ExtractDbException(exInfo.SqlException) as SqlException;
            if (sqle != null)
            {
                switch (sqle.Number)
                {
                case 17:
                    // SQL Server does not exist or access denied.
                case 4060:
                    // Invalid Database
                case 18456:
                    // Login Failed
                    return new DatabaseException(sqle.Message, sqle);
                case 547:
                    // ForeignKey Violation
                    return new ConstraintException(_ParseConstraintName(sqle.Message), sqle);
                case 1205:
                    // DeadLock Victim
                    return new DatabaseException(sqle.Message, sqle);
                case 2627:
                case 2601:
                    // Unique Index/Constriant Violation
                    return new ConstraintException(_ParseConstraintName(sqle.Message), sqle);
                default:
                    // throw a general DAL Exception
                    return new DatabaseException(sqle.Message, sqle);
                }
            }

            return SQLStateConverter.HandledNonSpecificException(
                       exInfo.SqlException,
                       exInfo.Message, exInfo.Sql);
        }

        /// <summary>
        /// Gets the name of a constraint.
        /// </summary>
        /// <value>The name of the constraint.</value>
        private static string _ParseConstraintName(string message)
        {
            var exp =
                new Regex(
                "^The\\sDELETE\\sstatement\\sconflicted\\swith\\sthe\\sREFERENCE\\sconstraint\\s[\"](?<value>\\w*)[\"]");
            MatchCollection matchList = exp.Matches(message);
            if (matchList.Count > 0)
            {
                return matchList[0].Groups["value"].Value;
            }

            exp = new Regex(@"^Violation\sof\sUNIQUE\sKEY\sconstraint\s['](?<value>\w*)[']");
            matchList = exp.Matches(message);
            if (matchList.Count > 0)
            {
                return matchList[0].Groups["value"].Value;
            }

            exp = new Regex(@"\w*unique index\s['](?<value>\w*)[']");
            matchList = exp.Matches(message);
            if (matchList.Count > 0)
            {
                return matchList[0].Groups["value"].Value;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}