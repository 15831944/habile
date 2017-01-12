using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using G = Geometry;

namespace Geometry_visual
{
    public partial class Drawing : Form
    {
        int s = 400;
        int scale = 10;
        int nullX = 0;
        int nullY = 0;

        public Drawing()
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(s + 40, s + 65);
            this.drawingArea.Size = new System.Drawing.Size(s, s);
        }
        
        private void drawingArea_Paint(object sender, PaintEventArgs e)
        {
            G.Region r = regioon();
            set_scale(r);
            draw_region(r, e);
        }

        private G.Region regioon()
        {
            List<G.Line> contours = new List<G.Line>();

            int of = -1500;
            int wi = 6510;
            int he = 2695;

            G.Point a = new G.Point(of, of);
            G.Point b = new G.Point(of + wi, of);
            G.Point c = new G.Point(of + wi, of + he);
            G.Point z = new G.Point(of + (wi / 2), of + he + 200);
            G.Point d = new G.Point(of, of + he);

            G.Point a2 = new G.Point(of + 2000, of + 1000);
            G.Point b2 = new G.Point(of + 2000, of + 2000);
            G.Point c2 = new G.Point(of + 4000, of + 2000);
            G.Point d2 = new G.Point(of + 4000, of + 1000);

            G.Line k = new G.Line(a, b);
            G.Line l = new G.Line(c, b);
            G.Line x = new G.Line(c, z);
            G.Line m = new G.Line(z, d);
            G.Line n = new G.Line(a, d);

            G.Line k2 = new G.Line(a2, b2);
            G.Line l2 = new G.Line(b2, c2);
            G.Line m2 = new G.Line(c2, d2);
            G.Line n2 = new G.Line(d2, a2);

            contours.Add(k);
            contours.Add(l);
            contours.Add(m);
            contours.Add(n);
            contours.Add(x);

            contours.Add(k2);
            contours.Add(l2);
            contours.Add(m2);
            contours.Add(n2);

            G.Region r = new G.Region(contours);

            return r;
        }

        private void set_scale(G.Region r)
        {
            double mix = r.Min_X;
            double miy = r.Min_Y;
            double mxx = r.Max_X;
            double mxy = r.Max_Y;

            double dx = Math.Abs(mxx - mix);
            double dy = Math.Abs(mxy - miy);

            double sc1 = dx / (s * 0.95);
            double sc2 = dy / (s * 0.95);

            scale = Convert.ToInt32(Math.Max(sc1, sc2));

            double nulloffsetX = (r.Min_X / scale) - ((s - (dx / scale)) / 2);
            double nulloffsetY = s + (r.Min_Y / scale) - ((s - (dy / scale)) / 2);

            nullX = Convert.ToInt32(nulloffsetX);
            nullY = Convert.ToInt32(nulloffsetY);
        }

        private void draw_region(G.Region r, PaintEventArgs e)
        {
            foreach (G.Edge ee in r.edges)
            {
                Pen ppp = new Pen(Color.DarkRed, 2);
                draw_line(ee.Line, ppp, e);
            }

            foreach (G.Corner cr in r.corners)
            {
                if (Math.Abs(Math.Abs(cr.Angle) - Math.PI / 2) < 0.01)
                {
                    Pen ppp = new Pen(Color.Green, 2);
                    draw_circle(cr.CP, ppp, e);
                }
                else if (Math.Abs(cr.Angle) > Math.PI)
                {
                    Pen ppp = new Pen(Color.Red, 2);
                    draw_circle(cr.CP, ppp, e);
                }
                else
                {
                    Pen ppp = new Pen(Color.Blue, 2);
                    draw_circle(cr.CP, ppp, e);
                }                
            }

            foreach (G.Edge ee in r.edges)
            {
                G.Point cp = ee.Line.getCenterPoint();
                G.Vector ov = ee.Line.getOffsetVector();
                G.Point op = cp.move(100, ov);

                Pen ppp = new Pen(Color.Red, 2);
                draw_point(op, ppp, e);
            }
        }

        private void draw_circle(G.Point p, Pen ppp, PaintEventArgs e)
        {
            int pointSize = 10;

            int x = (Convert.ToInt32(p.X) / scale) - nullX - pointSize / 2;
            int y = (-Convert.ToInt32(p.Y) / scale) + nullY - pointSize / 2;
            Point dp = new Point(x, y);

            e.Graphics.DrawEllipse(ppp, x, y, pointSize, pointSize);
        }

        private void draw_point(G.Point p, Pen ppp, PaintEventArgs e)
        {
            int pointSize = 4;

            int x = (Convert.ToInt32(p.X) / scale) - nullX - pointSize / 2;
            int y = (-Convert.ToInt32(p.Y) / scale) + nullY - pointSize / 2;
            Point dp = new Point(x, y);

            e.Graphics.DrawEllipse(ppp, x, y, pointSize, pointSize);
        }

        private void draw_line(G.Line ln, Pen ppp, PaintEventArgs e)
        {
            int x1 = (Convert.ToInt32(ln.Start.X) / scale) - nullX;
            int y1 = (-Convert.ToInt32(ln.Start.Y) / scale) + nullY;
            int x2 = (Convert.ToInt32(ln.End.X) / scale) - nullX;
            int y2 = (-Convert.ToInt32(ln.End.Y) / scale) + nullY;

            e.Graphics.DrawLine(ppp, x1, y1, x2, y2);
        }        
    }
}
