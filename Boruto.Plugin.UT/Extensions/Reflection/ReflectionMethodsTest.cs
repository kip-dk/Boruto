using Boruto.Attributes;
using Boruto.Extensions.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Plugin.UT.Extensions.Reflection
{

    [TestClass]
    public class ReflectionMethodsTest
    {
        [TestMethod]
        public void IsEntityTypeTest()
        {
            Assert.IsTrue(typeof(Boruto.Plugin.Entities.Account).IsEntityType());
            Assert.IsTrue(typeof(Target).IsEntityType());
            Assert.IsTrue(typeof(Preimage).IsEntityType());
            Assert.IsTrue(typeof(Merged).IsEntityType());
            Assert.IsTrue(typeof(Postimage).IsEntityType());
            Assert.IsFalse(typeof(object).IsEntityType());
        }

        [TestMethod]
        public void ResolveEntityTypeTest()
        {
            Assert.AreEqual(typeof(Microsoft.Xrm.Sdk.Entity), typeof(Microsoft.Xrm.Sdk.Entity).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));
            Assert.AreEqual(typeof(Entities.Account), typeof(Entities.Account).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));

            Assert.AreEqual(typeof(Target), typeof(Target).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));
            Assert.AreEqual(typeof(Preimage), typeof(Preimage).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));
            Assert.AreEqual(typeof(Merged), typeof(Merged).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));
            Assert.AreEqual(typeof(Postimage), typeof(Postimage).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));

            Assert.AreEqual(typeof(AccountMyInterface), typeof(IMyInterFace).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));
            Assert.AreEqual(typeof(ContactMyInterface), typeof(IMyInterFace).ResolveEntityType(Entities.Contact.EntityLogicalName, Consts.Assemblies));

            Assert.IsNull(typeof(IMyInterFace).ResolveEntityType(Entities.SystemUser.EntityLogicalName, Consts.Assemblies));

            Assert.AreEqual(typeof(Boruto.Plugin.Example.Entities.AccountStateChanged), typeof(Boruto.Plugin.Example.Entities.AccountStateChanged.IStatChanged).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));

            Assert.AreEqual(typeof(VerySpecialAccount), typeof(IVerySpecialAccount).ResolveEntityType(Entities.Account.EntityLogicalName, Consts.Assemblies));
        }


        [TestMethod]
        public void HasPublicDefaultConstructorTest()
        {
            Assert.IsTrue(typeof(Microsoft.Xrm.Sdk.Entity).HasPublicDefaultConstructor());
            Assert.IsTrue(typeof(Boruto.Plugin.Entities.Account).HasPublicDefaultConstructor());
            Assert.IsFalse(typeof(Boruto.Plugin.Example.Entities.AccountStateChanged).HasPublicDefaultConstructor());
        }


        [TestMethod]
        public void ResolveAttributesTest()
        {
            {
                var type = typeof(Boruto.Plugin.Entities.Account);
                var allTarget = type.ResolveAttributes(type, null, out bool all);
                Assert.IsNull(allTarget);
                Assert.IsTrue(all);
            }

            {
                var allTarget = typeof(IMyInterFace).ResolveAttributes(typeof(AccountMyInterface), null, out bool all);
                Assert.AreEqual(1, allTarget.Length);
                Assert.AreEqual("name", allTarget[0]);
                Assert.IsFalse(all);
            }

            {
                var allTarget = typeof(IMyInterFace).ResolveAttributes(typeof(ContactMyInterface), null, out bool all);
                Assert.AreEqual(1, allTarget.Length);
                Assert.AreEqual("fullname", allTarget[0]);
                Assert.IsFalse(all);
            }

            {
                var allTarget = typeof(IMyMerged).ResolveAttributes(typeof(AccountMyMerged), typeof(Boruto.Attributes.TargetFilterAttribute), out bool all);
                Assert.AreEqual(1, allTarget.Length);
                Assert.AreEqual("name", allTarget[0]);
                Assert.IsFalse(all);
            }

            {
                var allTarget = typeof(IMyMerged).ResolveAttributes(typeof(ContactMyMerged), typeof(Boruto.Attributes.TargetFilterAttribute), out bool all);
                Assert.AreEqual(1, allTarget.Length);
                Assert.AreEqual("fullname", allTarget[0]);
                Assert.IsFalse(all);
            }

            {
                var allTarget = typeof(IMyMerged).ResolveAttributes(typeof(AccountMyMerged), null, out bool all);
                Assert.AreEqual(2, allTarget.Length);
                Assert.AreEqual("name", allTarget.Where(r => r == "name").Single());
                Assert.AreEqual("description", allTarget.Where(r => r == "description").Single());
                Assert.IsFalse(all);
            }

            {
                var allTarget = typeof(IMyMerged).ResolveAttributes(typeof(ContactMyMerged), null, out bool all);
                Assert.AreEqual(2, allTarget.Length);
                Assert.AreEqual("fullname", allTarget.Where(r => r == "fullname").Single());
                Assert.AreEqual("description", allTarget.Where(r => r == "description").Single());
                Assert.IsFalse(all);
            }

        }

        #region multi target interface test
        public interface IMyInterFace : ITarget
        {
            string Name { get; }
        }

        public class AccountMyInterface : Entities.Account, IMyInterFace
        {
        }

        public class ContactMyInterface : Entities.Contact, IMyInterFace
        {
            [Microsoft.Xrm.Sdk.AttributeLogicalName("fullname")]
            public string Name => this.FullName;
        }
        #endregion

        #region multi merged interface test
        public interface IMyMerged : IMerged
        {
            [TargetFilter]
            string Name { get; }

            string Description { get; }
        }

        public class AccountMyMerged : Entities.Account, IMyMerged
        {
        }


        public class ContactMyMerged : Entities.Contact, IMyMerged
        {
            [Microsoft.Xrm.Sdk.AttributeLogicalName("fullname")]
            public string Name => this.FullName;
        }
        #endregion

        #region dummy impls for test purpose
        public class Target : ITarget
        {
            Guid IEntity.Id => throw new NotImplementedException();

            string IEntity.LogicalName => Entities.Account.EntityLogicalName;

            Microsoft.Xrm.Sdk.AttributeCollection IEntity.Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class Preimage : IPreImage
        {
            public Guid Id => throw new NotImplementedException();

            public string LogicalName => Entities.Account.EntityLogicalName;

            public Microsoft.Xrm.Sdk.AttributeCollection Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }

        public class Merged : IMerged
        {
            Guid IEntity.Id => throw new NotImplementedException();

            string IEntity.LogicalName => Entities.Account.EntityLogicalName;

            Microsoft.Xrm.Sdk.AttributeCollection IEntity.Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class Postimage : IPostImage
        {
            Guid IEntity.Id => throw new NotImplementedException();

            string IEntity.LogicalName => Entities.Account.EntityLogicalName;

            Microsoft.Xrm.Sdk.AttributeCollection IEntity.Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        }
        #endregion

        #region entity class that does not extend Account

        public interface IVerySpecialAccount: Boruto.ITarget
        {
            string Name { get; }
        }

        public class VerySpecialAccount : IVerySpecialAccount
        {
            public VerySpecialAccount(Microsoft.Xrm.Sdk.ITracingService traceService)
            {
            }

            private Microsoft.Xrm.Sdk.AttributeCollection attrs;
            string IVerySpecialAccount.Name => "Special";

            Guid IEntity.Id => Guid.Empty;

            string IEntity.LogicalName => Boruto.Plugin.Entities.Account.EntityLogicalName;

            Microsoft.Xrm.Sdk.AttributeCollection IEntity.Attributes { get => attrs; set => attrs = value; }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }
        }
        #endregion
    }
}
