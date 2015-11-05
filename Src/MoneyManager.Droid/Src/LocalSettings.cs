using System;
using System.Globalization;
using Android.App;
using Android.Content;
using Android.Preferences;
using Java.Lang;
using MoneyManager.Foundation.Interfaces;

namespace MoneyManager.Droid
{
    public class LocalSettings : ILocalSettings
    {
        private readonly object locker = new object();

        /// <summary>
        ///     Gets the current value or the default that you specify.
        /// </summary>
        /// <typeparam name="T">Vaue of t (bool, int, float, long, string)</typeparam>
        /// <param name="key">Key for settings</param>
        /// <param name="defaultValue">default value if not set</param>
        /// <returns>Value or default</returns>
        public T GetValueOrDefault<T>(string key, T defaultValue = default(T))
        {
            lock (locker)
            {
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    return GetValueOrDefaultCore(sharedPreferences, key, defaultValue);
                }
            }
        }

        private T GetValueOrDefaultCore<T>(ISharedPreferences sharedPreferences, string key, T defaultValue)
        {
            var typeOf = typeof (T);
            if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                typeOf = Nullable.GetUnderlyingType(typeOf);
            }

            object value = null;
            var typeCode = Type.GetTypeCode(typeOf);
            var resave = false;
            switch (typeCode)
            {
                case TypeCode.Decimal:
                    //Android doesn't have decimal in shared prefs so get string and convert
                    var savedDecimal = string.Empty;
                    try
                    {
                        savedDecimal = sharedPreferences.GetString(key, string.Empty);
                    }
                    catch (ClassCastException cce)
                    {
                        Console.WriteLine("Settings 1.5 change, have to remove key.");

                        try
                        {
                            Console.WriteLine("Attempting to get old value.");
                            savedDecimal =
                                sharedPreferences.GetLong(key,
                                    (long) Convert.ToDecimal(defaultValue, CultureInfo.InvariantCulture))
                                    .ToString();
                            Console.WriteLine("Old value has been parsed and will be updated and saved.");
                        }
                        catch (ClassCastException cce2)
                        {
                            Console.WriteLine("Could not parse old value, will be lost.");
                        }
                        Remove("key");
                        resave = true;
                    }
                    if (string.IsNullOrWhiteSpace(savedDecimal))
                        value = Convert.ToDecimal(defaultValue, CultureInfo.InvariantCulture);
                    else
                        value = Convert.ToDecimal(savedDecimal, CultureInfo.InvariantCulture);

                    if (resave)
                        AddOrUpdateValue(key, value);

                    break;
                case TypeCode.Boolean:
                    value = sharedPreferences.GetBoolean(key, Convert.ToBoolean(defaultValue));
                    break;
                case TypeCode.Int64:
                    value =
                        sharedPreferences.GetLong(key,
                            Convert.ToInt64(defaultValue, CultureInfo.InvariantCulture));
                    break;
                case TypeCode.String:
                    value = sharedPreferences.GetString(key, Convert.ToString(defaultValue));
                    break;
                case TypeCode.Double:
                    //Android doesn't have double, so must get as string and parse.
                    var savedDouble = string.Empty;
                    try
                    {
                        savedDouble = sharedPreferences.GetString(key, string.Empty);
                    }
                    catch (ClassCastException cce)
                    {
                        Console.WriteLine("Settings 1.5  change, have to remove key.");

                        try
                        {
                            Console.WriteLine("Attempting to get old value.");
                            savedDouble =
                                sharedPreferences.GetLong(key,
                                    (long) Convert.ToDouble(defaultValue, CultureInfo.InvariantCulture))
                                    .ToString();
                            Console.WriteLine("Old value has been parsed and will be updated and saved.");
                        }
                        catch (ClassCastException cce2)
                        {
                            Console.WriteLine("Could not parse old value, will be lost.");
                        }
                        Remove(key);
                        resave = true;
                    }
                    if (string.IsNullOrWhiteSpace(savedDouble))
                        value = Convert.ToDouble(defaultValue, CultureInfo.InvariantCulture);
                    else
                        value = Convert.ToDouble(savedDouble, CultureInfo.InvariantCulture);

                    if (resave)
                        AddOrUpdateValue(key, value);
                    break;
                case TypeCode.Int32:
                    value = sharedPreferences.GetInt(key,
                        Convert.ToInt32(defaultValue, CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Single:
                    value = sharedPreferences.GetFloat(key,
                        Convert.ToSingle(defaultValue, CultureInfo.InvariantCulture));
                    break;
                case TypeCode.DateTime:
                    var ticks = sharedPreferences.GetLong(key, -1);
                    if (ticks == -1)
                        value = defaultValue;
                    else
                        value = new DateTime(ticks);
                    break;
                default:

                    if (defaultValue is Guid)
                    {
                        var outGuid = Guid.Empty;
                        Guid.TryParse(sharedPreferences.GetString(key, Guid.Empty.ToString()), out outGuid);
                        value = outGuid;
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("Value of type {0} is not supported.",
                            value.GetType().Name));
                    }

                    break;
            }

            return null != value ? (T) value : defaultValue;
        }

        /// <summary>
        ///     Adds or updates a value
        /// </summary>
        /// <param name="key">key to update</param>
        /// <param name="value">value to set</param>
        /// <returns>True if added or update and you need to save</returns>
        public bool AddOrUpdateValue<T>(string key, T value)
        {
            var typeOf = typeof (T);
            if (typeOf.IsGenericType && typeOf.GetGenericTypeDefinition() == typeof (Nullable<>))
            {
                typeOf = Nullable.GetUnderlyingType(typeOf);
            }
            var typeCode = Type.GetTypeCode(typeOf);
            return AddOrUpdateValue(key, value, typeCode);
        }

        private bool AddOrUpdateValue(string key, object value, TypeCode typeCode)
        {
            lock (locker)
            {
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    using (var sharedPreferencesEditor = sharedPreferences.Edit())
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Decimal:
                                sharedPreferencesEditor.PutString(key,
                                    Convert.ToString(value, CultureInfo.InvariantCulture));
                                break;
                            case TypeCode.Boolean:
                                sharedPreferencesEditor.PutBoolean(key, Convert.ToBoolean(value));
                                break;
                            case TypeCode.Int64:
                                sharedPreferencesEditor.PutLong(key,
                                    Convert.ToInt64(value, CultureInfo.InvariantCulture));
                                break;
                            case TypeCode.String:
                                sharedPreferencesEditor.PutString(key, Convert.ToString(value));
                                break;
                            case TypeCode.Double:
                                sharedPreferencesEditor.PutString(key,
                                    Convert.ToString(value, CultureInfo.InvariantCulture));
                                break;
                            case TypeCode.Int32:
                                sharedPreferencesEditor.PutInt(key,
                                    Convert.ToInt32(value, CultureInfo.InvariantCulture));
                                break;
                            case TypeCode.Single:
                                sharedPreferencesEditor.PutFloat(key,
                                    Convert.ToSingle(value, CultureInfo.InvariantCulture));
                                break;
                            case TypeCode.DateTime:
                                sharedPreferencesEditor.PutLong(key, Convert.ToDateTime(value).Ticks);
                                break;
                            default:
                                if (value is Guid)
                                {
                                    sharedPreferencesEditor.PutString(key, ((Guid) value).ToString());
                                }
                                else
                                {
                                    throw new ArgumentException(string.Format("Value of type {0} is not supported.",
                                        value.GetType().Name));
                                }
                                break;
                        }

                        sharedPreferencesEditor.Commit();
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Removes a desired key from the settings
        /// </summary>
        /// <param name="key">Key for setting</param>
        public void Remove(string key)
        {
            lock (locker)
            {
                using (var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Application.Context))
                {
                    using (var sharedPreferencesEditor = sharedPreferences.Edit())
                    {
                        sharedPreferencesEditor.Remove(key);
                        sharedPreferencesEditor.Commit();
                    }
                }
            }
        }
    }
}