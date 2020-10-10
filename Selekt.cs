using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace Rota
{
	public class Selekt
	{
		public ModelVisual3D mv3_Model = null;
		public Matrix3D mx3_Undo = Matrix3D.Identity;

		public Ellipse el = new Ellipse();
		public Line lXY = new Line();
		public Line lZ = new Line();

		public Selekt(ModelVisual3D _mv3)
		{
			mv3_Model = _mv3;
			mx3_Undo = _mv3.Transform.Value;

			el.Fill = Brushes.Yellow;
			el.Stroke = Brushes.Black;
			el.Height = 10;
			el.Width = 10;

			/*
			_p.Stroke = Brushes.Black;
			_p.StrokeDashArray.Add(2);
			_p.StrokeThickness = 2;
			*/
				
			lXY.Stroke = Brushes.Gray;
			lXY.StrokeThickness = 1;
			//_lXY.StrokeDashArray.Add(2);

			lZ.Stroke = Brushes.Gray;
			lZ.StrokeThickness = 1;
			lZ.StrokeDashArray.Add(3);
				 
		}

		public void elSetPoz(double _x, double _y)
		{
			el.Margin = new Thickness(
				_x - (el.Width / 2),
				_y - (el.Height / 2),
				0, 0);

		}
		public void pathSetPoz(Point _p2d000, Point _p2dXY, Point _p2dXYZ)
		{
			/*
			PathSegmentCollection pc = new PathSegmentCollection();
			LineSegment ls1 = new LineSegment(_p2dXY, true);
			LineSegment ls2 = new LineSegment(_p2dXYZ, true);
			pc.Add(ls1);
			pc.Add(ls2);

			PathFigureCollection pfc = new PathFigureCollection();
			PathFigure pf = new PathFigure(_p2d000, pc, false);
			pfc.Add(pf);

			PathGeometry pg = new PathGeometry(pfc);
			_p.Data = pg;
			*/
			
			lXY.X1 = _p2d000.X;
			lXY.Y1 = _p2d000.Y;
			lXY.X2 = _p2dXY.X;
			lXY.Y2 = _p2dXY.Y;

			lZ.X1 = _p2dXY.X;
			lZ.Y1 = _p2dXY.Y;
			lZ.X2 = _p2dXYZ.X;
			lZ.Y2 = _p2dXYZ.Y;
		}
	}
}