using SR.EscrowBaseWeb.Auditing;
using SR.EscrowBaseWeb.Test.Base;
using Shouldly;
using Xunit;

namespace SR.EscrowBaseWeb.Tests.Auditing
{
    // ReSharper disable once InconsistentNaming
    public class NamespaceStripper_Tests: AppTestBase
    {
        private readonly INamespaceStripper _namespaceStripper;

        public NamespaceStripper_Tests()
        {
            _namespaceStripper = Resolve<INamespaceStripper>();
        }

        [Fact]
        public void Should_Stripe_Namespace()
        {
            var controllerName = _namespaceStripper.StripNameSpace("SR.EscrowBaseWeb.Web.Controllers.HomeController");
            controllerName.ShouldBe("HomeController");
        }

        [Theory]
        [InlineData("SR.EscrowBaseWeb.Auditing.GenericEntityService`1[[SR.EscrowBaseWeb.Storage.BinaryObject, SR.EscrowBaseWeb.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null]]", "GenericEntityService<BinaryObject>")]
        [InlineData("CompanyName.ProductName.Services.Base.EntityService`6[[CompanyName.ProductName.Entity.Book, CompanyName.ProductName.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[CompanyName.ProductName.Services.Dto.Book.CreateInput, N...", "EntityService<Book, CreateInput>")]
        [InlineData("SR.EscrowBaseWeb.Auditing.XEntityService`1[SR.EscrowBaseWeb.Auditing.AService`5[[SR.EscrowBaseWeb.Storage.BinaryObject, SR.EscrowBaseWeb.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],[SR.EscrowBaseWeb.Storage.TestObject, SR.EscrowBaseWeb.Core, Version=1.10.1.0, Culture=neutral, PublicKeyToken=null],]]", "XEntityService<AService<BinaryObject, TestObject>>")]
        public void Should_Stripe_Generic_Namespace(string serviceName, string result)
        {
            var genericServiceName = _namespaceStripper.StripNameSpace(serviceName);
            genericServiceName.ShouldBe(result);
        }
    }
}
