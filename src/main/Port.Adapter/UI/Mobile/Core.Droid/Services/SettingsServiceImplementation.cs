//MIT License

//Copyright(c) .NET Foundation and Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
// https://github.com/dotnet-architecture/eShopOnContainers
//
// Modifications copyright(C) 2018 ei8/Elmer Bool

using Android.App;
using Android.Content;
using Android.Preferences;
using System;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Droid.Services;

[assembly: Xamarin.Forms.Dependency(typeof(SettingsServiceImplementation))]
namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Droid.Services
{
    public class SettingsServiceImplementation : ISettingsServiceImplementation
    {
        #region Internal Implementation

        readonly object _locker = new object();

        ISharedPreferences GetSharedPreference()
        {
            return PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
        }

        bool AddOrUpdateValueInternal<T>(string key, T value)
        {
            if (Android.App.Application.Context == null)
                return false;

            if (value == null)
            {
                Remove(key);
                return true;
            }

            var type = typeof(T);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }
            var typeCode = Type.GetTypeCode(type);

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    using (var editor = sharedPrefs.Edit())
                    {
                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                                editor.PutBoolean(key, Convert.ToBoolean(value));
                                break;
                            case TypeCode.String:
                                editor.PutString(key, Convert.ToString(value));
                                break;
                            default:
                                throw new ArgumentException($"Value of type {typeCode} is not supported.");
                        }
                        editor.Commit();
                    }
                }
            }
            return true;
        }

        T GetValueOrDefaultInternal<T>(string key, T defaultValue = default(T))
        {
            if (Android.App.Application.Context == null)
                return defaultValue;

            if (!Contains(key))
                return defaultValue;

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    var type = typeof(T);
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }

                    object value = null;
                    var typeCode = Type.GetTypeCode(type);
                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            value = sharedPrefs.GetBoolean(key, Convert.ToBoolean(defaultValue));
                            break;
                        case TypeCode.String:
                            value = sharedPrefs.GetString(key, Convert.ToString(defaultValue));
                            break;
                        default:
                            throw new ArgumentException($"Value of type {typeCode} is not supported.");
                    }

                    return null != value ? (T)value : defaultValue;
                }
            }
        }

        bool Contains(string key)
        {
            if (Android.App.Application.Context == null)
                return false;

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    if (sharedPrefs == null)
                        return false;
                    return sharedPrefs.Contains(key);
                }
            }
        }

        #endregion

        #region ISettingsServiceImplementation

        public bool AddOrUpdateValue(string key, bool value) => AddOrUpdateValueInternal(key, value);

        public bool AddOrUpdateValue(string key, string value) => AddOrUpdateValueInternal(key, value);

        public bool GetValueOrDefault(string key, bool defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public string GetValueOrDefault(string key, string defaultValue) => GetValueOrDefaultInternal(key, defaultValue);

        public void Remove(string key)
        {
            if (Android.App.Application.Context == null)
                return;

            lock (_locker)
            {
                using (var sharedPrefs = GetSharedPreference())
                {
                    using (var editor = sharedPrefs.Edit())
                    {
                        editor.Remove(key);
                        editor.Commit();
                    }
                }
            }
        }

        #endregion
    }
}
