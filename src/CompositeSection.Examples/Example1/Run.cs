using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompositeSection.Lib;


namespace CompositeSection.Examples.Example1
{
    public class Run
    {
        public void DoRun()
        {
            Material conc, steel;

            var w = 0.500;//width 50cm
            var h = 0.500;//height 50 cm
            var c = 0.05;//cover 5cm
            var phi = 0.01;//bar diameter 1cm


            {
                conc = CompositeSection.Lib.Materials.ParabolicLinearConcrete.CreateEc2(20);//20mpa fc
                steel = CompositeSection.Lib.Materials.PerfectElasticPlastic.Create(350);//350 mpa yield
            }

            var sec = new Section();

           


            {
                var concRect = new SurfaceElement();

                concRect.Points = new PointCollection();


                concRect.Points.Add(
                    new Point(-w / 2, -h / 2),
                    new Point(w / 2, -h / 2),
                    new Point(w / 2, h / 2),
                    new Point(-w / 2, h / 2),
                    new Point(-w / 2, -h / 2));//

                concRect.ForegroundMaterial = conc;
                concRect.BackgroundMaterial = null;
                sec.SurfaceElements.Add(concRect);
            }

            {
                var d = c + phi / 2;

                var steelpoints = new Point[] {
                    new Point(-w / 2+d, -h / 2+d),
                    new Point(w / 2-d, -h / 2+d),
                    new Point(w / 2-d, h / 2-d),
                    new Point(-w / 2+d, h / 2-d)};

                foreach (var pt in steelpoints)
                {
                    var fe = new FiberElement();

                    fe.Area = Math.PI * phi * phi / 4;

                    fe.ForegroundMaterial = steel;
                    fe.BackgroundMaterial = conc;
                    //sinse each fiber is inside the concrete,
                    //background material is concrete
                    //and should be subtraced from result

                    sec.FiberElements.Add(fe);
                }
            }


            var str = new StrainProfile();
            
            str.E0 = -0.003;//axial strain 
            str.Ky = str.Kz = 0;//curvature

            var forces = sec.GetSectionForces(str);

            System.Console.WriteLine("Section forces: Fx: {0:0.0} Ton, My: {1}, Mz: {2}", forces.Nx / -1e4, forces.My, forces.Mz);
            //axial force is ~ 500 Ton

            System.Console.ReadKey();
        }
    }
}
