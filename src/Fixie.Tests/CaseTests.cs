﻿using Should;

namespace Fixie.Tests
{
    public class CaseTests
    {
        public void ShouldBeNamedAfterTheUnderlyingMethod()
        {
            var @case = Case("Returns");

            @case.Name.ShouldEqual("Fixie.Tests.CaseTests.Returns");
        }

        public void ShouldIncludeParmeterValuesInNameWhenTheUnderlyingMethodHasParameters()
        {
            var @case = Case("Parameterized", 123, true, 'a', "with \"quotes\"", "long \"string\" gets truncated", null, this);

            @case.Name.ShouldEqual("Fixie.Tests.CaseTests.Parameterized(123, True, 'a', \"with \\\"quotes\\\"\", \"long \\\"string\\\" g\"..., null, Fixie.Tests.CaseTests)");
        }

        public void ShouldIncludeResolvedGenericArgumentsInNameWhenTheUnderlyingMethodIsGeneric()
        {
            Case("Generic", 123, true, "a", "b")
                .Name.ShouldEqual("Fixie.Tests.CaseTests.Generic<System.Boolean, System.String>(123, True, \"a\", \"b\")");

            Case("Generic", 123, true, 1, null)
                .Name.ShouldEqual("Fixie.Tests.CaseTests.Generic<System.Boolean, System.Object>(123, True, 1, null)");

            Case("Generic", 123, 1.23m, "a", null)
                .Name.ShouldEqual("Fixie.Tests.CaseTests.Generic<System.Decimal, System.String>(123, 1.23, \"a\", null)");
        }

        public void ShouldInferAppropriateClassGivenCaseMethod()
        {
            var methodDeclaredInChildClass = new Case(typeof(SampleChildTestClass).GetInstanceMethod("TestMethodDefinedWithinChildClass"));
            methodDeclaredInChildClass.Class.ShouldEqual(typeof(SampleChildTestClass));

            var methodDeclaredInParentClass = new Case(typeof(SampleParentTestClass).GetInstanceMethod("TestMethodDefinedWithinParentClass"));
            methodDeclaredInParentClass.Class.ShouldEqual(typeof(SampleParentTestClass));

            var parentMethodInheritedByChildClass = new Case(typeof(SampleChildTestClass).GetInstanceMethod("TestMethodDefinedWithinParentClass"));
            parentMethodInheritedByChildClass.Class.ShouldEqual(typeof(SampleChildTestClass));
        }

        class SampleParentTestClass
        {
            public void TestMethodDefinedWithinParentClass()
            {
            }
        }

        class SampleChildTestClass : SampleParentTestClass
        {
            public void TestMethodDefinedWithinChildClass()
            {
            }
        }

        static Case Case(string methodName, params object[] parameters)
        {
            return new Case(typeof(CaseTests).GetInstanceMethod(methodName), parameters);
        }

        void Returns()
        {
        }

        void Throws()
        {
            throw new FailureException();
        }

        void Parameterized(int i, bool b, char ch, string s1, string s2, object obj, CaseTests complex)
        {
        }

        void Generic<T1, T2>(int i, T1 t1, T2 t2a, T2 t2b)
        {
        }
    }
}
