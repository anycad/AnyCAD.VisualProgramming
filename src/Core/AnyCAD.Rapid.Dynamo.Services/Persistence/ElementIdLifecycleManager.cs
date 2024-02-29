using DynamoServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnyCAD.Rapid.Dynamo.Services.Persistence
{
    /// <summary>
    /// ElementIdLifecycleManager
    /// </summary>
    internal class ElementIdLifecycleManager
    {
        private static Object mSingletonMutex = new object();
        private static ElementIdLifecycleManager? mInstance;

        private Dictionary<ulong, List<object>> mWrappers;

        private ElementIdLifecycleManager()
        {
            mWrappers = new();
        }

        public static void DisposeInstance()
        {
            lock (mSingletonMutex)
            {
                if (mInstance != null)
                {
                    mInstance = null;
                }
            }
        }

        /// <summary>
        /// Get the LifecycleManager for the specific type
        /// WARNING: This is only a singleton for a given TypeArg
        /// </summary>
        /// <returns></returns>
        public static ElementIdLifecycleManager Instance
        {
            get
            {
                lock (mSingletonMutex)
                {
                    if (mInstance == null)
                    {
                        mInstance = new();
                    }
                    return mInstance;
                }
            }
        }

        /// <summary>
        /// Register a new dependency between an element ID and a wrapper
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="wrapper"></param>
        public void RegisterAsssociation(ulong elementId, object wrapper)
        {
            List<object> existingWrappers;
            if (mWrappers.TryGetValue(elementId, out existingWrappers))
            {
                //ID already existed, check we're not over adding
                Validity.Assert(!existingWrappers.Contains(wrapper),
                    "Lifecycle manager alert: registering the same AnyCAD Element Wrapper twice"
                    + " {6528305F}");
            }
            else
            {
                existingWrappers = new();
                mWrappers.Add(elementId, existingWrappers);
            }

            existingWrappers.Add(wrapper);
            //if (!ownerDeleted.ContainsKey(elementId))
            //{
            //    ownerDeleted.Add(elementId, false);
            //}
        }

        /// <summary>
        /// Remove an association between an element ID and 
        /// </summary>
        /// <param name="elementId"></param>
        /// <param name="wrapper"></param>
        /// <returns>The number of remaining associations</returns>
        public int UnRegisterAssociation(ulong elementId, object wrapper)
        {
            List<object> existingWrappers;
            if (mWrappers.TryGetValue(elementId, out existingWrappers))
            {
                //ID already existed, check we're not over adding
                if (existingWrappers.Contains(wrapper))
                {
                    int index = existingWrappers.FindIndex((x) => object.ReferenceEquals(x, wrapper));
                    existingWrappers.RemoveAt(index);
                    if (existingWrappers.Count == 0)
                    {
                        mWrappers.Remove(elementId);
                        // ownerDeleted.Remove(elementId);
                        return 0;
                    }
                    else
                    {
                        return existingWrappers.Count;
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "Attempting to remove a wrapper that wasn't there registered");
                }

            }
            else
            {
                //The Id didn't exist

                throw new InvalidOperationException(
                    "Attempting to remove a wrapper, but there were no ids registered");
            }

        }

        /// <summary>
        /// Get the number of wrappers that are registered
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetRegisteredCount(ulong id)
        {
            if (!mWrappers.ContainsKey(id))
            {
                return 0;
            }
            else
            {
                return mWrappers[id].Count;
            }

        }
    }
}
