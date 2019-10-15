using System;
using System.Collections.Generic;
using System.Text;
using CookieManager;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Cells.SOLib
{
    /// <summary>
	/// Implementation of <see cref="ICookieManager" /> 
	/// </summary>
	public class DefaultCookieManager : ICookieManager
    {
        private readonly ICookie _cookie;

        public DefaultCookieManager(ICookie cookie)
        {
            this._cookie = cookie;
        }


        public bool Contains(string key)
        {
            return _cookie.Contains(key);
        }

        /// <summary>
        /// Get the value or set the value if specified key is expire
        /// </summary>
        /// <typeparam name="T">TSource</typeparam>
        /// <param name="key">Key</param>
        /// <param name="acquirer">action to execute</param>
        /// <param name="expireTime">Expire time</param>
        /// <returns>TSource object</returns>
        public T GetOrSet<T>(string key, Func<T> acquirer, int? expireTime = default(int?))
        {
            if (_cookie.Contains(key))
            {
                //get the existing value
                return GetExisting<T>(key);
            }
            else
            {
                var value = acquirer();
                this.Set(key, value, expireTime);
                return value;
            }

            return GetExisting<T>(key);
        }

        private T GetExisting<T>(string key)
        {
            var value = _cookie.Get(key);

            if (string.IsNullOrEmpty(value))
                return default(T);
            if (typeof(T).ToString() == typeof(string).ToString())
            {
                object v = value;
                return (T)v;
            }
            else
                return JsonConvert.DeserializeObject<T>(value);

        }

        /// <summary>
        /// remove the key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            _cookie.Remove(key);
        }

        /// <summary>
        /// set the value 
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="expireTime"></param>
        public void Set(string key, object value, int? expireTime = default(int?))
        {
            if (value is string)
                _cookie.Set(key, value.ToString(), expireTime);
            else
                _cookie.Set(key, JsonConvert.SerializeObject(value), expireTime);

        }


        /// <summary>
        /// get the value of specified key
        /// </summary>
        /// <typeparam name="T">T object</typeparam>
        /// <param name="key">Key</param>
        /// <returns>T object</returns>
        public T Get<T>(string key)
        {
            return GetExisting<T>(key);
        }

        public void Set(string key, object value, CookieOptions option)
        {
            if (value is string)
                _cookie.Set(key, value.ToString(), option);
            else
                _cookie.Set(key, JsonConvert.SerializeObject(value), option);

        }

        public T GetOrSet<T>(string key, Func<T> acquirer, CookieOptions option)
        {
            if (_cookie.Contains(key))
            {
                //get the existing value
                return GetExisting<T>(key);
            }
            else
            {
                var value = acquirer();
                this.Set(key, value, option);

                return value;
            }

            return GetExisting<T>(key);
        }
    }
}
