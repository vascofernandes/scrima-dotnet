﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Scrima.OData {
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Messages {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Scrima.OData.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to This service does not contain a collection named &apos;{0}&apos;.
        /// </summary>
        internal static string CollectionNameInvalid {
            get {
                return ResourceManager.GetString("CollectionNameInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The value for OData query $count must be either &apos;true&apos; or &apos;false&apos;.
        /// </summary>
        internal static string CountRawValueInvalid {
            get {
                return ResourceManager.GetString("CountRawValueInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The value for OData query &apos;{0}&apos; must be a non-negative numeric value..
        /// </summary>
        internal static string IntRawValueInvalid {
            get {
                return ResourceManager.GetString("IntRawValueInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to If specified, the odata.metadata value in the Accept header must be &apos;none&apos;, &apos;minimal&apos; or &apos;full&apos;.
        /// </summary>
        internal static string ODataMetadataValueInvalid {
            get {
                return ResourceManager.GetString("ODataMetadataValueInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The value for OData query &apos;{0}&apos; cannot be empty..
        /// </summary>
        internal static string ODataQueryCannotBeEmpty {
            get {
                return ResourceManager.GetString("ODataQueryCannotBeEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to If specified, the $orderby direction must be either &apos;asc&apos; or &apos;desc&apos;..
        /// </summary>
        internal static string OrderByPropertyRawValueInvalid {
            get {
                return ResourceManager.GetString("OrderByPropertyRawValueInvalid", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The specified $top value exceeds the maximum value of &apos;{0}&apos; permitted by this service..
        /// </summary>
        internal static string TopValueExceedsMaxAllowed {
            get {
                return ResourceManager.GetString("TopValueExceedsMaxAllowed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The $filter query option could not be parsed by this service..
        /// </summary>
        internal static string UnableToParseFilter {
            get {
                return ResourceManager.GetString("UnableToParseFilter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The $select query option could not be parsed by this service..
        /// </summary>
        internal static string UnableToParseSelect {
            get {
                return ResourceManager.GetString("UnableToParseSelect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; operator is not a valid operator..
        /// </summary>
        internal static string UnknownOperator {
            get {
                return ResourceManager.GetString("UnknownOperator", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The query option &apos;{0}&apos; is not a valid query option. Valid query options are $count, $expand, $filter, $format, $orderby, $search, $select, $skip, $skiptoken and $top.
        /// </summary>
        internal static string UnknownQueryOption {
            get {
                return ResourceManager.GetString("UnknownQueryOption", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; function is not implemented by this service..
        /// </summary>
        internal static string UnsupportedFunction {
            get {
                return ResourceManager.GetString("UnsupportedFunction", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to If specified, the OData-IsolationLevel must be &apos;Snapshot&apos;.
        /// </summary>
        internal static string UnsupportedIsolationLevel {
            get {
                return ResourceManager.GetString("UnsupportedIsolationLevel", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Query nodes of type &apos;{0}&apos; are not supported..
        /// </summary>
        internal static string UnsupportedNodeType {
            get {
                return ResourceManager.GetString("UnsupportedNodeType", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to This service only supports OData 4.0.
        /// </summary>
        internal static string UnsupportedODataVersion {
            get {
                return ResourceManager.GetString("UnsupportedODataVersion", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; operator is not implemented by this service..
        /// </summary>
        internal static string UnsupportedOperator {
            get {
                return ResourceManager.GetString("UnsupportedOperator", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The query option &apos;{0}&apos; is not implemented by this service..
        /// </summary>
        internal static string UnsupportedQueryOption {
            get {
                return ResourceManager.GetString("UnsupportedQueryOption", resourceCulture);
            }
        }
    }
}
