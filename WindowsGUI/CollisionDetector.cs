using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using static System.Windows.Forms.DataFormats;

namespace WindowsGUI
{

    

    public static class CollisionDetector
    {
        static Random random = new Random();
        static string clickPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Resources","click.mp3");
        // WindowsGUI\bin\Debug\net10.0-windows\Resources\click.mp3
        // D:\NSCC\semester3\advprog\assignments\WindowsGUI\WindowsGUI\Resources\click.mp3
        static string wowPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "wow.mp3");
        static MciPlayer click = new MciPlayer("D:\\NSCC\\semester3\\advprog\\assignments\\WindowsGUI\\WindowsGUI\\Resources\\click.mp3", "click");
        static MciPlayer wow = new MciPlayer("D:\\NSCC\\semester3\\advprog\\assignments\\WindowsGUI\\WindowsGUI\\Resources\\wow.mp3", "woww");

        #region Collision Detection

        // project all vertices of shape onto given axis
        private static void Project(PointF[] vertices, PointF axis, out float min, out float max)
        {
            // init bounds to extremes
            min = float.MaxValue;
            max = float.MinValue;

            // find "dot product" of each vertex & the axis
            // a · b = |a||b|cos(θ)    where θ is the angle between the vectors
            // https://www.mathsisfun.com/algebra/vectors-dot-product.html
            // this determines how far along the axis each vertex lies
            foreach (PointF vertex in vertices)
            {
                float projection = vertex.X * axis.X + vertex.Y * axis.Y;

                // update min & max projection values
                min = Math.Min(min, projection);
                max = Math.Max(max, projection);
            }
        }

        // determine whether 2 projected intervals overlap
        // if they dont overlap, u found a seperating axis (ie. not colliding)
        private static bool Overlaps(float minA, float maxA, float minB, float maxB)
        {
            return maxA >= minB && maxB >= minA;
        }

        // find perpendicular vector to edge (the "normal")
        private static PointF GetNormal(PointF edge)
        {
            return new PointF(-edge.Y, edge.X);
        }

        //checks whether 2 shapes overlap along the perpendicular axes from the edges of the first shape

        private static bool CheckAxes(PointF[] verticesA, PointF[] verticesB, ref float minPenetration, ref PointF collisionNormal)
        {
            for (int i = 0; i < verticesA.Length; i++)
            {
                PointF current = verticesA[i];
                PointF next = verticesA[(i + 1) % verticesA.Length];

                PointF edge = new PointF(
                    next.X - current.X,
                    next.Y - current.Y
                );

                PointF axis = GetNormal(edge);

                // Normalise the axis so penetration is measured in pixels.
                float length = (float)Math.Sqrt(
                    axis.X * axis.X + axis.Y * axis.Y
                );

                if (length == 0)
                    continue;

                axis = new PointF(axis.X / length, axis.Y / length);

                // Project both shapes onto the axis.
                Project(verticesA, axis, out float minA, out float maxA);
                Project(verticesB, axis, out float minB, out float maxB);

                // No overlap means a separating axis has been found.
                if (!Overlaps(minA, maxA, minB, maxB))
                    return false;

                // Calculate the amount of overlap along this axis.
                float overlap = Math.Min(maxA - minB, maxB - minA);

                // Keep track of the smallest overlap.
                if (overlap < minPenetration)
                {
                    minPenetration = overlap;
                    collisionNormal = axis;
                }
            }

            return true;
        }

        public struct CollisionInfo
        {
            public PointF Normal;
            public float Penetration;
            public PointF ContactPoint;
        }

        private static PointF GetSupportPoint(PointF[] vertices, PointF direction)
        {
            PointF best = vertices[0];

            float bestProjection = best.X * direction.X + best.Y * direction.Y;

            foreach (PointF vertex in vertices)
            {
                float projection = vertex.X * direction.X + vertex.Y * direction.Y;

                if (projection > bestProjection)
                {
                    bestProjection = projection;
                    best = vertex;
                }
            }

            return best;
        }

        // determines whether shapes are colliding
        // https://en.wikipedia.org/wiki/Hyperplane_separation_theorem
        // https://programmerart.weebly.com/separating-axis-theorem.html

        public static bool SATCollision(
            Shape a,
            Shape b,
            out CollisionInfo collision)
        {
            // Initialise collision information.
            collision = new CollisionInfo();

            // Retrieve vertices of both shapes.
            PointF[] verticesA = a.GetVertices();
            PointF[] verticesB = b.GetVertices();

            // Initialise minimum penetration and collision normal.
            float minPenetration = float.MaxValue;
            PointF normal = PointF.Empty;

            // Check axes generated from both shapes.
            // If a separating axis is found, there is no collision.
            if (!CheckAxes(verticesA, verticesB, ref minPenetration, ref normal))
                return false;

            if (!CheckAxes(verticesB, verticesA, ref minPenetration, ref normal))
                return false;

            // Find the direction between the centres of both shapes.
            PointF direction = new PointF(
                b.Position.X - a.Position.X,
                b.Position.Y - a.Position.Y
            );

            // Ensure the collision normal points from A towards B.
            float dot = normal.X * direction.X + normal.Y * direction.Y;

            if (dot < 0)
            {
                normal = new PointF(-normal.X, -normal.Y);
            }

            // Find the support points of both shapes.
            PointF supportA = GetSupportPoint(verticesA, normal);

            PointF supportB = GetSupportPoint(
                verticesB,
                new PointF(-normal.X, -normal.Y)
            );

            // Estimate the contact point using the midpoint
            // between the two support points.
            PointF contactPoint = new PointF(
                (supportA.X + supportB.X) / 2,
                (supportA.Y + supportB.Y) / 2
            );

            // Populate the collision information.
            collision = new CollisionInfo
            {
                Normal = normal,
                Penetration = minPenetration,
                ContactPoint = contactPoint
            };

            // A collision was detected.
            return true;
        }
        #endregion

        #region Math Helper Functions

        // vector x vector
        // https://www.mathsisfun.com/algebra/vectors-cross-product.html
        private static float Cross(PointF a, PointF b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        // Angular velocity x radius vector
        private static PointF Cross(float angularVelocity, PointF r)
        {
            return new PointF(
                (-angularVelocity * r.Y),
                (angularVelocity * r.X)
            );
        }

        private static PointF Subtract(PointF a, PointF b)
        {
            return new PointF(a.X - b.X, a.Y - b.Y);
        }

        private static PointF Add(PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }

        private static PointF Multiply(PointF v, float scalar)
        {
            return new PointF(
                v.X * scalar,
                v.Y * scalar
            );
        }
        #endregion

        public static void ResolveCollision(Shape a, Shape b, CollisionInfo collision)
        {

            //Vector from each center to contact point
            PointF ra = Subtract(collision.ContactPoint, a.Position);
            PointF rb = Subtract(collision.ContactPoint, b.Position);

            // Velocity at contact point, including rotation
            PointF velocityA = Add(a.Velocity, Cross(a.AngularVelocity, ra));
            PointF velocityB = Add(b.Velocity, Cross(b.AngularVelocity, rb));

            PointF relativeVelocity = Subtract(velocityB, velocityA);

            // velocity in direction of other object
            float velocityAlongNormal =
                relativeVelocity.X * collision.Normal.X +
                relativeVelocity.Y * collision.Normal.Y;

            float restitution = 1.0f; // <--- Bounciness multiplier

            float raCrossN = Cross(ra, collision.Normal);
            float rbCrossN = Cross(rb, collision.Normal);

            float inverseMassA = a.InverseMass;
            float inverseMassB = b.InverseMass;

            float inverseInertiaA = a.InverseInertia;
            float inverseInertiaB = b.InverseInertia;

            // calc impulse magnitude
            float denominator =
                inverseMassA + inverseMassB +
                raCrossN * raCrossN * inverseInertiaA +
                rbCrossN * rbCrossN * inverseInertiaB;

            if (denominator <= 0) return;

            // impulse force
            float j = -(1 + restitution) * velocityAlongNormal / denominator;

            // collision impulse vector
            PointF impulse = Multiply(collision.Normal, j);

            //Apply linear impulse
            a.Velocity = Subtract(
                a.Velocity,
                Multiply(impulse, inverseMassA)
            );
            b.Velocity = Add(
                b.Velocity,
                Multiply(impulse, inverseMassB)
            );

            // Apply angular impulse
            a.AngularVelocity -=
                inverseInertiaA * Cross(ra, impulse);

            b.AngularVelocity +=
                inverseInertiaB * Cross(rb, impulse);


            // Positional correction
            float percent = 0.8f; // Percentage of overlap to correct
            float slop = 0.01f;   // Allow a tiny amount of penetration

            float correctionMagnitude =
                Math.Max(collision.Penetration - slop, 0.0f) /
                (inverseMassA + inverseMassB) * percent;

            PointF correction = Multiply(
                collision.Normal,
                correctionMagnitude
            );

            // Move shapes apart according to their inverse masses
            a.Position = Subtract(
                a.Position,
                Multiply(correction, inverseMassA)
            );

            b.Position = Add(
                b.Position,
                Multiply(correction, inverseMassB)
            );


            float impactA = a.Mass * Math.Abs(
                velocityA.X * collision.Normal.X +
                velocityA.Y * collision.Normal.Y
            );

            float impactB = b.Mass * Math.Abs(
                velocityB.X * collision.Normal.X +
                velocityB.Y * collision.Normal.Y
            );
            //Debug.WriteLine("ImpactA: " + impactA + "\tImpactB: " + impactB);

            int whichColor = random.Next(1, 4);

            if (impactA <= impactB)
            {
                switch (whichColor)
                {
                    case 1:
                        a.Color = Color.FromArgb(255, random.Next(128, 256), random.Next(0, 256), random.Next(0, 256));
                        break;
                    case 2:
                        a.Color = Color.FromArgb(255, random.Next(0, 256), random.Next(128, 256), random.Next(0, 256));
                        break;
                    case 3:
                        a.Color = Color.FromArgb(255, random.Next(0, 256), random.Next(0, 256), random.Next(128, 256));
                        break;
                }
            }
            else
            {
                switch (whichColor)
                {
                    case 1:
                        b.Color = Color.FromArgb(255, random.Next(128, 256), random.Next(0, 256), random.Next(0, 256));
                        break;
                    case 2:
                        b.Color = Color.FromArgb(255, random.Next(0, 256), random.Next(128, 256), random.Next(0, 256));
                        break;
                    case 3:
                        b.Color = Color.FromArgb(255, random.Next(0, 256), random.Next(0, 256), random.Next(128, 256));
                        break;
                }
            }

            
            if (velocityAlongNormal > 0.0f)
            {
                if (Math.Abs(j) >= 100000)
                {
                    wow.PlayFromStart();
                    Debug.WriteLine("wowww");

                }
                else
                {
                    //click.PlayFromStart();
                    //Debug.WriteLine("else reached");
                }
            }

        }
        
    }
}
