#region license
// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region acknowledgements
//
// Code reduced from http://github.com/castleprojectcontrib/Castle.Windsor.Lifestyles
//
#endregion

using System.Reflection;
using System.Web;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Lifestyle.Scoped;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;

namespace TestBase.Example.WebApi.Windsor.Windsor3HybridWebLifeStyleManager
{
    /// <summary>
    /// Abstract hybrid lifestyle manager, with two underlying lifestyles
    /// </summary>
    /// <typeparam name="M1">Primary lifestyle manager</typeparam>
    /// <typeparam name="M2">Secondary lifestyle manager</typeparam>
    public abstract class HybridLifestyleManager<M1, M2> : AbstractLifestyleManager
        where M1 : ILifestyleManager, new()
        where M2 : ILifestyleManager, new()
    {
        protected readonly M1 lifestyle1 = new M1();
        protected readonly M2 lifestyle2 = new M2();

        public override void Dispose()
        {
            lifestyle1.Dispose();
            lifestyle2.Dispose();
        }

        public override void Init(IComponentActivator componentActivator, IKernel kernel, ComponentModel model)
        {
            lifestyle1.Init(componentActivator, kernel, model);
            lifestyle2.Init(componentActivator, kernel, model);
        }

        public override bool Release(object instance)
        {
            var r1 = lifestyle1.Release(instance);
            var r2 = lifestyle2.Release(instance);
            return r1 || r2;
        }

        public abstract override object Resolve(CreationContext context, IReleasePolicy releasePolicy);
    }

    public static class ComponentRegistrationExtensionForHybridLifestyle
    {
        /// <summary>
        /// One component instance per web request, or if HttpContext is not available, transient.
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public static ComponentRegistration<S> HybridPerWebRequestTransient<S>(this LifestyleGroup<S> @group) where S : class
        {
            return @group.Scoped<HybridPerWebRequestTransientScopeAccessor>();
        }        
    }

    public class HybridPerWebRequestTransientScopeAccessor : HybridPerWebRequestScopeAccessor
    {
        public HybridPerWebRequestTransientScopeAccessor() :
            base(new TransientScopeAccessor()) { }
    }
    public class HybridPerWebRequestScopeAccessor : IScopeAccessor
    {
        private readonly IScopeAccessor webRequestScopeAccessor = new WebRequestScopeAccessor();
        private readonly IScopeAccessor secondaryScopeAccessor;

        public HybridPerWebRequestScopeAccessor(IScopeAccessor secondaryScopeAccessor)
        {
            this.secondaryScopeAccessor = secondaryScopeAccessor;
        }

        public ILifetimeScope GetScope(CreationContext context)
        {
            if (HttpContext.Current != null && PerWebRequestLifestyleModuleUtils.IsInitialized)
                return webRequestScopeAccessor.GetScope(context);
            return secondaryScopeAccessor.GetScope(context);
        }

        public void Dispose()
        {
            webRequestScopeAccessor.Dispose();
            secondaryScopeAccessor.Dispose();
        }
    }
    public class PerWebRequestLifestyleModuleUtils
    {
        private static readonly FieldInfo InitializedFieldInfo = typeof(PerWebRequestLifestyleModule).GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);

        public static bool IsInitialized
        {
            get
            {
                return (bool)InitializedFieldInfo.GetValue(null);
            }
        }
    }
    public class TransientScopeAccessor : IScopeAccessor
    {
        public ILifetimeScope GetScope(CreationContext context)
        {
            return new DefaultLifetimeScope();
        }

        public void Dispose()
        {
        }
    }
}