using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model.geom
{
    public class TestTopoTriangle
    {
        int faildTests;
        void TestRightAngle()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(5,5,0));
            TopoVertex v4 = model.addVertex(new RHVector3(0, 0, -5));
            TopoVertex v5 = model.addVertex(new RHVector3(10, 0, -5));
            TopoVertex v6 = model.addVertex(new RHVector3(5, 0, 5));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model,v1,v2,v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v4, v5, v6));
            if (!t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestRightAngle");
            }
        }
        void TestSharedEdge()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(5, 5, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(0, 0, -5));
            TopoVertex v5 = model.addVertex(new RHVector3(10, 0, -5));
            TopoVertex v6 = model.addVertex(new RHVector3(5, 0, 5));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v1, v2, v6));
            if (t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestSharedEdge");
            }
        }
        void TestSharedPointIntersect()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(5, 5, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(0, 0, -5));
            TopoVertex v5 = model.addVertex(new RHVector3(10, 0, -5));
            TopoVertex v6 = model.addVertex(new RHVector3(5, 0, 5));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v1, v5, v6));
            if (!t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestSharedPointIntersect");
            }
        }
        void TestInplaneInside()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(10, 10, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(1, 1, 0));
            TopoVertex v5 = model.addVertex(new RHVector3(7, 1, 0));
            TopoVertex v6 = model.addVertex(new RHVector3(1, 7, 0));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v4, v5, v6));
            if (!t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestInplaneInside");
            }
        }
        void TestInplaneOutside()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(10, 10, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(11, 1, 0));
            TopoVertex v5 = model.addVertex(new RHVector3(17, 1, 0));
            TopoVertex v6 = model.addVertex(new RHVector3(11, 7, 0));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v4, v5, v6));
            if (t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestInplaneOutside");
            }
        }
        void TestInplaneInsideSameEdge()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(10, 10, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(1, 1, 0));
            TopoVertex v5 = model.addVertex(new RHVector3(7, 1, 0));
            TopoVertex v6 = model.addVertex(new RHVector3(0, -10, 0));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v1, v2, v6));
            if (t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestInplaneInsideSameEdge");
            }
        }
        void TestInplaneInsideSameEdgeIntersects()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(10, 10, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(1, 1, 0));
            TopoVertex v5 = model.addVertex(new RHVector3(7, 1, 0));
            TopoVertex v6 = model.addVertex(new RHVector3(5, 3, 0));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v1, v2, v6));
            if (!t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestInplaneInsideSameEdgeIntersects");
            }
        }
        void TestInplaneSameVertex()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(10, 10, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(10, -1, 0));
            TopoVertex v5 = model.addVertex(new RHVector3(7, 1, 0));
            TopoVertex v6 = model.addVertex(new RHVector3(1, -7, 0));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v1, v4, v6));
            if (t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestInplaneSameVertex");
            }
        }
        void TestInplaneSameVertexIntersects()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(0, 0, 0));
            TopoVertex v2 = model.addVertex(new RHVector3(10, 0, 0));
            TopoVertex v3 = model.addVertex(new RHVector3(10, 10, 0));
            TopoVertex v4 = model.addVertex(new RHVector3(8, 1, 0));
            TopoVertex v5 = model.addVertex(new RHVector3(7, 1, 0));
            TopoVertex v6 = model.addVertex(new RHVector3(1, -7, 0));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v1, v4, v6));
            if (!t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestInplaneSameVertexIntersects");
            }
        }
        void TestInplane3D_1()
        {
            TopoModel model = new TopoModel();
            TopoVertex v1 = model.addVertex(new RHVector3(3.67848944664001,-2.6547646522522,1.38814495312454E-14));
            TopoVertex v2 = model.addVertex(new RHVector3(1.62981510162354,-1.05116808414459,1.83297828141877E-14));
            TopoVertex v3 = model.addVertex(new RHVector3(2.29873323440552,-0.79055267572403,2.11497486191092E-14));
            TopoVertex v4 = model.addVertex(new RHVector3(1.63205575942993,-1.05116808414459,2.78849697113037));
            TopoVertex v5 = model.addVertex(new RHVector3(0.916237592697144,-1.1297744512558,1.83297828141877E-14));
            TopoVertex v6 = model.addVertex(new RHVector3(1.38571500778198,-1.07829427719116,2.67316389083862));
            TopoTriangle t1 = model.AddTriangle(new TopoTriangle(model, v1, v2, v3));
            TopoTriangle t2 = model.AddTriangle(new TopoTriangle(model, v4, v5, v6));
            if (t1.Intersects(t2))
            {
                faildTests++;
                Console.WriteLine("Failed test:TestInplane3D_1");
            }
        }
        public void RunTests()
        {
            faildTests = 0;
            TestRightAngle();
            TestSharedEdge();
            TestSharedPointIntersect();
            TestInplaneInside();
            TestInplaneInsideSameEdge();
            TestInplaneInsideSameEdgeIntersects();
            TestInplaneOutside();
            TestInplaneSameVertex();
            TestInplaneSameVertexIntersects();
            TestInplane3D_1();
            Console.WriteLine("Failed:" + faildTests);
        }
    }
}
