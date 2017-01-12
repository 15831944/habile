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

namespace Logic_Reinf
{
    public partial class Drawing_Box : Form
    {
        int s = 800;
        int scale = 1;
        int nullX = 0;
        int nullY = 0;
        G.Region r;
        List<G.Line> re;

        bool flag = true;

        List<G.Edge> notE;
        List<G.Corner> notC;
        List<G.Edge> setE;
        List<G.Corner> setC;

        public Drawing_Box(G.Region reg, List<G.Line> reinf)
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(s + 40, s + 65);
            this.pb_visual.Size = new System.Drawing.Size(s, s);
            r = reg;
            re = reinf;

            flag = true;
        }

        public Drawing_Box(G.Region reg, List<G.Edge> emptyEdgesDebug, List<G.Corner> emptyCornersDebug, List<G.Edge> setEdgesDebug, List<G.Corner> setCornerDebug, List<G.Line> reinf)
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(s + 40, s + 65);
            this.pb_visual.Size = new System.Drawing.Size(s, s);
            r = reg;
            re = reinf;

            notE = emptyEdgesDebug;
            notC = emptyCornersDebug;
            setE = setEdgesDebug;
            setC = setCornerDebug;

            flag = false;
        }

        private void pb_visual_Paint(object sender, PaintEventArgs e)
        {
            set_scale(r);
            if (flag)
            {
                draw_region(r, re, e);
            }
            else
            {
                draw_set_edges(notE, notC, setE, setC, re, e);
            }
        }

        private void set_scale(G.Region r)
        {
            double mix = r.Min_X;
            double miy = r.Min_Y;
            double mxx = r.Max_X;
            double mxy = r.Max_Y;

            double dx = Math.Abs(mxx - mix);
            double dy = Math.Abs(mxy - miy);

            double sc1 = dx / (s * 0.90);
            double sc2 = dy / (s * 0.90);

            scale = Convert.ToInt32(Math.Max(sc1, sc2));

            double nulloffsetX = (r.Min_X / scale) - ((s - (dx / scale)) / 2);
            double nulloffsetY = s + (r.Min_Y / scale) - ((s - (dy / scale)) / 2);

            nullX = Convert.ToInt32(nulloffsetX);
            nullY = Convert.ToInt32(nulloffsetY);
        }

        private void draw_region(G.Region r, List<G.Line> reinf, PaintEventArgs e)
        {
            foreach (G.Line re in reinf)
            {
                Pen ppp = new Pen(Color.Cyan, 2);
                draw_line(re, ppp, e);
            }

            // EDGES
            foreach (G.Edge ee in r.edges)
            {
                Pen ppp = new Pen(Color.DarkRed, 2);
                draw_line(ee.Line, ppp, e);
            }

            //CENTER POINT OFFSET
            foreach (G.Edge ee in r.edges)
            {
                G.Point cp = ee.Line.getCenterPoint();
                G.Vector ov = ee.Line.getOffsetVector();
                G.Point op = cp.move(20, ov);

                Pen ppp = new Pen(Color.Red, 2);
                draw_point(op, ppp, e);
            }

            //CORNERS
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
        }

        private void draw_set_edges(List<G.Edge> emptyEdgesDebug, List<G.Corner> emptyCornersDebug, List<G.Edge> setEdgesDebug, List<G.Corner> setCornerDebug, List<G.Line> reinf, PaintEventArgs e)
        {
            foreach (G.Line re in reinf)
            {
                Pen ppp = new Pen(Color.Cyan, 2);
                draw_line(re, ppp, e);
            }

            // EMPTY EDGES
            foreach (G.Edge ee in emptyEdgesDebug)
            {
                Pen ppp = new Pen(Color.Red, 3);
                draw_line(ee.Line, ppp, e);
            }

            // EMPTY CORNERS
            foreach (G.Corner cr in emptyCornersDebug)
            {
                Pen ppp = new Pen(Color.Red, 3);
                draw_circle(cr.CP, ppp, e, 20);
            }

            // SET EDGES
            foreach (G.Edge ee in setEdgesDebug)
            {
                Pen ppp = new Pen(Color.DarkGreen, 2);
                draw_line(ee.Line, ppp, e);
            }

            // SET CORNERS
            foreach (G.Corner cr in setCornerDebug)
            {
                Pen ppp = new Pen(Color.DarkGreen, 2);
                draw_circle(cr.CP, ppp, e);
            }
        }

        private void draw_circle(G.Point p, Pen ppp, PaintEventArgs e, int pointSize = 10)
        {
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
