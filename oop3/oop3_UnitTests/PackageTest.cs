/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections;
using System.Text;
using System.Reflection;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using japroc_company.oop3;

namespace oop3_UnitTests
{
    [TestClass()]
    public class PackageTest
    {
        [TestMethod()]
        public void CreateInstance()
        {
            oop3Package package = new oop3Package();
        }

        [TestMethod()]
        public void IsIVsPackage()
        {
            oop3Package package = new oop3Package();
            Assert.IsNotNull(package as IVsPackage, "The object does not implement IVsPackage");
        }

        [TestMethod()]
        public void SetSite()
        {
            // Create the package
            IVsPackage package = new oop3Package() as IVsPackage;
            Assert.IsNotNull(package, "The object does not implement IVsPackage");

            // Create a basic service provider
            OleServiceProvider serviceProvider = OleServiceProvider.CreateOleServiceProviderWithBasicServices();

            // Add site support to register editor factory
            BaseMock registerEditor = oop3_UnitTests.EditorTests.RegisterEditorsServiceMock.GetRegisterEditorsInstance();
            serviceProvider.AddService(typeof(SVsRegisterEditors), registerEditor, false);

            // Site the package
            Assert.AreEqual(0, package.SetSite(serviceProvider), "SetSite did not return S_OK");

            // Unsite the package
            Assert.AreEqual(0, package.SetSite(null), "SetSite(null) did not return S_OK");
        }
    }
}
