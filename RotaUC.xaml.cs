using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Input;

namespace Rota
{
	public partial class RotaUC
	{
		private RotaKamera rotKam = new RotaKamera(); // glavna klasa za kameru
		private mojiModeli mojModel = new mojiModeli(); // embedani modeli

		public RotaKamera kamera
		{
			get { return rotKam; }
		}

		// sadrzi ModelVisual3D
		private System.Collections.ArrayList FLAG_SelKolekcija = new System.Collections.ArrayList();

		//private Matrix3D FLAG_mx3TransformCancel = Matrix3D.Identity;
		//private Point FLAG_p2dTransformKanvasKoordOrigin = new Point(0, 0);
		private Point FLAG_p2dSelektBoxKanvasMousePozOrigin = new Point(0, 0);
		private Point FLAG_p2dKanvasKoordStara = new Point(0, 0);
		private ModelVisual3D FLAG_mv3_global = new ModelVisual3D();
		private DirectionalLight FLAG_DirLightKamera = new DirectionalLight();
		private Rectangle FLAG_rectBoxSelect = new Rectangle();
		private Ellipse FLAG_elipGrupSelekt = new Ellipse();
		private Point3D FLAG_p3dGrupSelektCentar = new Point3D(0, 0, 0);

		private bool FLAG_bLinijePozicije = true;
		//private bool FLAG_bKameraAnimating = false;

		private bool FLAG_KameraPerspektivnaAktivna = false; // kod stavljanja novih objekta u perspektivnoj kameri:
															 //private bool FLAG_bNamjestamVrijednostiUKontroli = true; // kada treba namjestiti vrijednosti u sliderima bez da njihovi eventi ista rade
		private bool FLAG_KameraOrbit = false;
		private bool FLAG_KameraPan = false;
		private bool FLAG_KameraZoom = false;

		private bool FLAG_SelektBoxAktiv = false;
		private bool FLAG_SelektPostoji
		{
			get
			{
				if (FLAG_SelKolekcija.Count > 0) { return true; }
				else { return false; }
			}
		}
		private bool FLAG_TransformObjektAktiv
		{
			get
			{
				if (FLAG_TransformObjektMove || FLAG_TransformObjektRotate || FLAG_TransformObjektScale)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		private bool FLAG_TransformObjektMove = false;
		private bool FLAG_TransformObjektRotate = false;
		private bool FLAG_TransformObjektScale = false;

		private bool FLAG_TransformConstrainX = false;
		private bool FLAG_TransformConstrainY = false;
		private bool FLAG_TransformConstrainZ = false;


		// dependancy propertiji m, r, s, x, y, z
		/*
		public static readonly DependencyProperty FLAG_kanvasTipka_MProperty =
			DependencyProperty.Register("FLAG_kanvasTipka_M", typeof(bool), typeof(RotaUC), new UIPropertyMetadata(false, new PropertyChangedCallback(RotaUC.HandlePropertyChange_TransformacijaBool)));
		public static readonly DependencyProperty FLAG_kanvasTipka_RProperty =
			DependencyProperty.Register("FLAG_kanvasTipka_R", typeof(bool), typeof(RotaUC), new UIPropertyMetadata(false, new PropertyChangedCallback(RotaUC.HandlePropertyChange_TransformacijaBool)));
		public static readonly DependencyProperty FLAG_kanvasTipka_SProperty =
			DependencyProperty.Register("FLAG_kanvasTipka_S", typeof(bool), typeof(RotaUC), new UIPropertyMetadata(false, new PropertyChangedCallback(RotaUC.HandlePropertyChange_TransformacijaBool)));
		public static readonly DependencyProperty FLAG_kanvasTipka_XProperty =
			DependencyProperty.Register("FLAG_kanvasTipka_X", typeof(bool), typeof(RotaUC), new UIPropertyMetadata(false, new PropertyChangedCallback(RotaUC.HandlePropertyChange_TransformacijaBool)));
		public static readonly DependencyProperty FLAG_kanvasTipka_YProperty =
			DependencyProperty.Register("FLAG_kanvasTipka_Y", typeof(bool), typeof(RotaUC), new UIPropertyMetadata(false, new PropertyChangedCallback(RotaUC.HandlePropertyChange_TransformacijaBool)));
		public static readonly DependencyProperty FLAG_kanvasTipka_ZProperty =
			DependencyProperty.Register("FLAG_kanvasTipka_Z", typeof(bool), typeof(RotaUC), new UIPropertyMetadata(false, new PropertyChangedCallback(RotaUC.HandlePropertyChange_TransformacijaBool)));
		public static void HandlePropertyChange_TransformacijaBool(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			Console.WriteLine(source);
			Console.WriteLine("  " + e.OldValue +"->"+e.NewValue);
			Console.WriteLine(((RotaUC)source).FLAG_kanvasTipka_M + ", " + ((RotaUC)source).FLAG_kanvasTipka_R + ", " + ((RotaUC)source).FLAG_kanvasTipka_S);
			//((RotaUC)source).EVENT_TransformacijaBoolFlagPromjena();
		}


		public bool FLAG_kanvasTipka_M
		{
			get { return (bool)GetValue(FLAG_kanvasTipka_MProperty); }
			set { SetValue(FLAG_kanvasTipka_MProperty, value); }
		}
		public bool FLAG_kanvasTipka_R
		{
			get { return (bool)GetValue(FLAG_kanvasTipka_RProperty); }
			set { SetValue(FLAG_kanvasTipka_RProperty, value); }
		}
		public bool FLAG_kanvasTipka_S
		{
			get { return (bool)GetValue(FLAG_kanvasTipka_SProperty); }
			set { SetValue(FLAG_kanvasTipka_SProperty, value); }
		}
		public bool FLAG_kanvasTipka_X
		{
			get { return (bool)GetValue(FLAG_kanvasTipka_XProperty); }
			set { SetValue(FLAG_kanvasTipka_XProperty, value); }
		}
		public bool FLAG_kanvasTipka_Y
		{
			get { return (bool)GetValue(FLAG_kanvasTipka_YProperty); }
			set { SetValue(FLAG_kanvasTipka_YProperty, value); }
		}
		public bool FLAG_kanvasTipka_Z
		{
			get { return (bool)GetValue(FLAG_kanvasTipka_ZProperty); }
			set { SetValue(FLAG_kanvasTipka_ZProperty, value); }
		}
		 */
		/*
		private void EVENT_TransformacijaBoolFlagPromjena()
		{
			string status = "";

			if (FLAG_kanvasTipka_M)
			{
				status = "Move";
			}
			else if (FLAG_kanvasTipka_R)
			{
				status = "Rotate";
			}
			else if (FLAG_kanvasTipka_S)
			{
				status = "Scale";
			}

			if (FLAG_kanvasTipka_M || FLAG_kanvasTipka_R || FLAG_kanvasTipka_S)
			{
				if (FLAG_kanvasTipka_X)
				{
					status += " - X";
					label_status_transform.Foreground = Brushes.Red;
				}
				else if (FLAG_kanvasTipka_Y)
				{
					status += " - Y";
					label_status_transform.Foreground = Brushes.Green;
				}
				else if (FLAG_kanvasTipka_Z)
				{
					status += " - Z";
					label_status_transform.Foreground = Brushes.Blue;
				}
				else
				{
					label_status_transform.Foreground = Brushes.Black;
				}
			}


			label_status_transform.Content = status;
			
		}
		*/
		public RotaUC()
		{
			this.InitializeComponent();
			/*
			label_status_sel.Content = "";
			label_status_transform.Content = "";
			label_status_koord.Content = "";
			*/

			FLAG_elipGrupSelekt.Fill = Brushes.Orange;
			FLAG_elipGrupSelekt.Stroke = Brushes.Black;
			FLAG_elipGrupSelekt.Height = 15;
			FLAG_elipGrupSelekt.Width = 15;

			rotKam.EVENT_PromjenaParametaraKamere += new EventHandler(rotrot_EVENT_PromjenaParametaraKamere);

			SolidColorBrush b = new SolidColorBrush(Colors.Purple);
			b.Opacity = 0.3;
			FLAG_rectBoxSelect.Fill = b;
			FLAG_rectBoxSelect.Stroke = new SolidColorBrush(Colors.White);

			FLAG_DirLightKamera.Color = Colors.White;
			FLAG_DirLightKamera.Direction = new Vector3D(-1, 1, -1);

			Model3DGroup m3g_global = new Model3DGroup();
			// osi
			m3g_global.Children.Add(mojModel.MODEL_OsX);
			m3g_global.Children.Add(mojModel.MODEL_OsY);
			m3g_global.Children.Add(mojModel.MODEL_OsZ);
			// svjetlo
			m3g_global.Children.Add(FLAG_DirLightKamera);

			FLAG_mv3_global.Content = m3g_global;

			////////////////////////////

			ModelVisual3D mv3 = citajIzXamla_v3d("kockaPuc.xaml");
			Viewport3D_Prikaz.Children.Add(mv3);

			////////////////////////////

			Viewport3D_Prikaz.Children.Add(FLAG_mv3_global);
			Viewport3D_Prikaz.Camera = rotKam.KameraOrt;

			//FLAG_bNamjestamVrijednostiUKontroli = false;

			// otvara
			rotKam.KameraZoom_dp = 10;

		}

		// selekcija
		private ModelVisual3D rayTest(Point _p2d)
		{
			// selekcija
			RayMeshGeometry3DHitTestResult rayMeshResult =
				VisualTreeHelper.HitTest(
				Viewport3D_Prikaz,
				_p2d)
				as RayMeshGeometry3DHitTestResult;

			//VisualTreeHelper.HitTest(
			//HitTestParameters3D
			//Console.WriteLine("rez: " + rayMeshResult);

			//	EmissiveMaterial myEmissiveMaterial = new EmissiveMaterial(new SolidColorBrush(Colors.Black));
			//	myEmissiveMaterial.Brush.Opacity = 0.3;
			//Console.WriteLine(rayMeshResult);
			if (rayMeshResult != null)
			{


				if (rayMeshResult.VisualHit.GetType() == typeof(ModelVisual3D))
				{
					ModelVisual3D mv3 = (ModelVisual3D)rayMeshResult.VisualHit;
					return mv3;
				}
				else if (rayMeshResult.VisualHit.GetType() == typeof(Viewport2DVisual3D))
				{
					ModelVisual3D mv3 = (ModelVisual3D)VisualTreeHelper.GetParent((Viewport2DVisual3D)rayMeshResult.VisualHit);

					return mv3;

					//Viewport2DVisual3D v2v = (Viewport2DVisual3D)rayMeshResult.VisualHit;

				}

				//Console.WriteLine("nasao: " + mv3);
			}
			return null;
		}
		private int modelSelectedIndexOd(ModelVisual3D _mv3)
		{
			for (int i = 0; i < FLAG_SelKolekcija.Count; i++)
			{
				if (((Selekt)FLAG_SelKolekcija[i]).mv3_Model == _mv3)
				{
					return i;
				}
			}

			return -1;
		}

		private void selektBoxBegin()
		{
			FLAG_SelektBoxAktiv = true;

			FLAG_p2dSelektBoxKanvasMousePozOrigin = System.Windows.Input.Mouse.GetPosition(Canvas_Prikaz);
			FLAG_rectBoxSelect.Margin = new Thickness(FLAG_p2dSelektBoxKanvasMousePozOrigin.X, FLAG_p2dSelektBoxKanvasMousePozOrigin.Y, 0, 0);
			FLAG_rectBoxSelect.Height = 1;
			FLAG_rectBoxSelect.Width = 1;

			if (!Canvas_Prikaz.Children.Contains(FLAG_rectBoxSelect))
			{
				Canvas_Prikaz.Children.Add(FLAG_rectBoxSelect);
			}
		}
		private void selektBoxEnd()
		{
			Canvas_Prikaz.Children.Remove(FLAG_rectBoxSelect);
			FLAG_SelektBoxAktiv = false;
		}
		private void selektBoxApply()
		{
			//Console.WriteLine(FLAG_rectBoxSelect.Width + ", " + FLAG_rectBoxSelect.Height);

			// ako je kutija malena radi hit test
			if ((FLAG_rectBoxSelect.Width <= 2) || (FLAG_rectBoxSelect.Height <= 2))
			{
				ModelVisual3D _mv3Selekt = rayTest(FLAG_p2dSelektBoxKanvasMousePozOrigin);
				if (_mv3Selekt != null)
				{
					// ne diraj glavne osi ili frozen objekte
					if (!(ModelVisual3D.Equals(_mv3Selekt, FLAG_mv3_global) || (_mv3Selekt.Content.IsFrozen)))
					{
						// shift prosiruje/smanjuje postojecu selekciju
						if (System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift))
						{
							int index = modelSelectedIndexOd(_mv3Selekt);

							if (index == -1)
							{
								modelSelectedDodaj(_mv3Selekt);
							}
							else
							{
								modelSelectedMakni(_mv3Selekt);
							}
						}
						else
						{
							modelSelectedMakniSve();
							modelSelectedDodaj(_mv3Selekt);
						}
					}
				}
				else
				{
					if (!System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift))
					{
						modelSelectedMakniSve();
					}
				}
			}

			// provjerava je li centar kojeg objekta unutar kutije
			else
			{
				if (!System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftShift))
				{
					modelSelectedMakniSve();
				}

				foreach (ModelVisual3D _mv3 in Viewport3D_Prikaz.Children)
				{
					if (ModelVisual3D.Equals(_mv3, FLAG_mv3_global) || (_mv3.Content.IsFrozen))
					{
						// ne diraj glavne osi ili frozen objekte
						continue;
					}
					Point3D p3dObjSrediste3D = new Point3D(_mv3.Transform.Value.OffsetX, _mv3.Transform.Value.OffsetY, _mv3.Transform.Value.OffsetZ);
					Point p2dObjSredisteKanvas2D = rotKam.Projektor_2DKanvasVia3DViewport(p3dObjSrediste3D, FLAG_KameraPerspektivnaAktivna);
					Point p2dKontrolKoord = rotKam.Projektor_KursorPozVia2DKanvas(Canvas_Prikaz.ActualWidth, Canvas_Prikaz.ActualHeight, p2dObjSredisteKanvas2D);

					// ako je unutar kutije
					if ((p2dKontrolKoord.X > FLAG_rectBoxSelect.Margin.Left) && (p2dKontrolKoord.X < FLAG_rectBoxSelect.Margin.Left + FLAG_rectBoxSelect.Width))
					{
						if ((p2dKontrolKoord.Y > FLAG_rectBoxSelect.Margin.Top) && (p2dKontrolKoord.Y < FLAG_rectBoxSelect.Margin.Top + FLAG_rectBoxSelect.Height))
						{
							if (modelSelectedIndexOd(_mv3) == -1)
							{
								modelSelectedDodaj(_mv3);
							}
						}
					}
				}
			}
			selektBoxEnd();

			/*
			// hittest za svaki pixel kutije, tocno ali presporo!
			//Console.WriteLine(FLAG_rectBox.Margin.Left + ";;" +FLAG_rectBox.Width);
			for (double i = FLAG_rectBox.Margin.Left; i < FLAG_rectBox.Margin.Left + FLAG_rectBox.Width; i++)
			{
				for (double j = FLAG_rectBox.Margin.Top; j < FLAG_rectBox.Margin.Top + FLAG_rectBox.Height; j++)
				{
					//Console.WriteLine(p2d);

					ModelVisual3D mv3 = rayTest(new Point(i, j));
					
					if (mv3 != null)
					{
						if (!FLAG_SelKolekcija.Contains(mv3))
						{
							modelSelectedDodaj(mv3);
						}
					}
					
				}
			}
			*/
		}

		private void modelSelectedDodaj(ModelVisual3D _mv3)
		{
			//	if (!FLAG_SelKolekcija.Contains(_mv3))

			Selekt sel = new Selekt(_mv3);

			FLAG_SelKolekcija.Add(sel);

			modelSelectedKruzicUpdate();
		}
		private void modelSelectedMakni(ModelVisual3D _mv3)
		{
			int index = modelSelectedIndexOd(_mv3);

			if (index > -1)
			{
				Canvas_Prikaz.Children.Remove(((Selekt)FLAG_SelKolekcija[index]).el);
				Canvas_Prikaz.Children.Remove(((Selekt)FLAG_SelKolekcija[index]).lXY);
				Canvas_Prikaz.Children.Remove(((Selekt)FLAG_SelKolekcija[index]).lZ);

				FLAG_SelKolekcija.RemoveAt(index);

				modelSelectedKruzicUpdate();
			}
		}
		private void modelSelectedMakniSve()
		{
			FLAG_SelKolekcija.Clear();
			modelSelectedKruzicUpdate();
		}
		private void modelSelectedKruzicUpdate()
		{
			Canvas_Prikaz.BeginInit();

			Canvas_Prikaz.Children.Clear();
			Vector3D v3dSelektGrupaCentar_tmp = new Vector3D(0, 0, 0);

			for (int i = 0; i < FLAG_SelKolekcija.Count; i++)
			{
				Selekt sel = (Selekt)FLAG_SelKolekcija[i];
				ModelVisual3D _mv3 = sel.mv3_Model;

				Point3D p3dObjSrediste3D = new Point3D(_mv3.Transform.Value.OffsetX, _mv3.Transform.Value.OffsetY, _mv3.Transform.Value.OffsetZ);

				Point p2dObjSredisteXYZMousePoz = rotKam.Projektor_KursorPozVia3DViewport(
					p3dObjSrediste3D,
					FLAG_KameraPerspektivnaAktivna,
					Canvas_Prikaz.ActualWidth,
					Canvas_Prikaz.ActualHeight);
				// sredisnji kruzic
				sel.elSetPoz(p2dObjSredisteXYZMousePoz.X, p2dObjSredisteXYZMousePoz.Y);

				// linije prostorne pozicije
				// linije idu od (0,0,0) do (x,y,0) pa do (x,y,z)
				if (FLAG_bLinijePozicije)
				{
					Point3D p3dObjSredisteXY = new Point3D(p3dObjSrediste3D.X, p3dObjSrediste3D.Y, 0);
					Point p2dObjSredisteXYMousePoz = rotKam.Projektor_KursorPozVia3DViewport(
						p3dObjSredisteXY,
						FLAG_KameraPerspektivnaAktivna,
						Canvas_Prikaz.ActualWidth,
						Canvas_Prikaz.ActualHeight);
					Point p2d000MousePoz = rotKam.Projektor_KursorPozVia3DViewport(
						new Point3D(0, 0, 0),
						FLAG_KameraPerspektivnaAktivna,
						Canvas_Prikaz.ActualWidth,
						Canvas_Prikaz.ActualHeight);
					sel.pathSetPoz(p2d000MousePoz, p2dObjSredisteXYMousePoz, p2dObjSredisteXYZMousePoz);
				}
				// kruzic se sakrije kad je udaljensot manja od 0
				double udaljenostSelObjodKamere = rotKam.Projektor_OrtoUdaljenostTockeOdKamere(p3dObjSrediste3D);
				if (udaljenostSelObjodKamere >= 0)
				{
					Canvas_Prikaz.Children.Add(sel.el);
					if (FLAG_bLinijePozicije)
					{
						Canvas_Prikaz.Children.Add(sel.lXY);
						Canvas_Prikaz.Children.Add(sel.lZ);
					}

				}
				v3dSelektGrupaCentar_tmp += (Vector3D)p3dObjSrediste3D;
			}

			// grup selekt kruzic
			if (FLAG_SelektPostoji)
			{
				FLAG_p3dGrupSelektCentar = (Point3D)(v3dSelektGrupaCentar_tmp / FLAG_SelKolekcija.Count);

				Point p2dSelektGrupaPoz = rotKam.Projektor_KursorPozVia3DViewport(
					FLAG_p3dGrupSelektCentar,
					FLAG_KameraPerspektivnaAktivna,
					Canvas_Prikaz.ActualWidth,
					Canvas_Prikaz.ActualHeight);

				FLAG_elipGrupSelekt.Margin = new Thickness(
					p2dSelektGrupaPoz.X - (FLAG_elipGrupSelekt.Width / 2),
					p2dSelektGrupaPoz.Y - (FLAG_elipGrupSelekt.Height / 2), 0, 0);

				double udaljenostSelGrupodKamere = rotKam.Projektor_OrtoUdaljenostTockeOdKamere(FLAG_p3dGrupSelektCentar);
				if (udaljenostSelGrupodKamere >= 0)
				{
					Canvas_Prikaz.Children.Add(FLAG_elipGrupSelekt);
				}
			}
			Canvas_Prikaz.EndInit();
			/*
			if (FLAG_SelKolekcija.Count >= 1)
			{
				label_status_sel.Content = "Selected: " + FLAG_SelKolekcija.Count.ToString() + ";";
			}
			else
			{
				label_status_sel.Content = "";
			}
			*/
		}
		private void modelSelectedTransformBegin()
		{
			// podaci za undo
			for (int i = 0; i < FLAG_SelKolekcija.Count; i++)
			{
				((Selekt)FLAG_SelKolekcija[i]).mx3_Undo = ((Selekt)FLAG_SelKolekcija[i]).mv3_Model.Transform.Value;
			}
		}
		private void modelSelectedTransformEnd()
		{
			FLAG_TransformObjektMove = false;
			FLAG_TransformObjektRotate = false;
			FLAG_TransformObjektScale = false;
			FLAG_TransformConstrainX = false;
			FLAG_TransformConstrainY = false;
			FLAG_TransformConstrainZ = false;
		}
		private void modelSelectedTransformCancel()
		{
			for (int i = 0; i < FLAG_SelKolekcija.Count; i++)
			{
				MatrixTransform3D mt3 = new MatrixTransform3D(((Selekt)FLAG_SelKolekcija[i]).mx3_Undo);
				((Selekt)FLAG_SelKolekcija[i]).mv3_Model.Transform = mt3;
			}
			modelSelectedTransformEnd();
			modelSelectedKruzicUpdate();
		}


		private void xyzOsiVidljive(bool x, bool y, bool z)
		{
			Model3DGroup m3g = (Model3DGroup)FLAG_mv3_global.Content;

			if (x)
			{
				if (!m3g.Children.Contains(mojModel.MODEL_OsX))
				{
					m3g.Children.Add(mojModel.MODEL_OsX);
				}
			}
			else
			{
				m3g.Children.Remove(mojModel.MODEL_OsX);
			}
			if (y)
			{
				if (!m3g.Children.Contains(mojModel.MODEL_OsY))
				{
					m3g.Children.Add(mojModel.MODEL_OsY);
				}
			}
			else
			{
				m3g.Children.Remove(mojModel.MODEL_OsY);
			}
			if (z)
			{
				if (!m3g.Children.Contains(mojModel.MODEL_OsZ))
				{
					m3g.Children.Add(mojModel.MODEL_OsZ);
				}
			}
			else
			{
				m3g.Children.Remove(mojModel.MODEL_OsZ);
			}

		}
		private Point kanvasTrenutnaKoordinataMisaOrto()
		{
			Point p2dKanvasTrenutnaKoordinataMisa = rotKam.Projektor_2DKanvasViaKanvasKursorPoz(
					Canvas_Prikaz.ActualWidth,
					Canvas_Prikaz.ActualHeight,
					System.Windows.Input.Mouse.GetPosition(Canvas_Prikaz),
					//	FLAG_bPerspektivnaKameraAktivna
					false
					);

			return p2dKanvasTrenutnaKoordinataMisa;

		}
		private Point kanvasTrenutnaKoordinataMisaOrto(Point tocka)
		{
			Point p2dKanvasTrenutnaKoordinataMisa = rotKam.Projektor_2DKanvasViaKanvasKursorPoz(
					Canvas_Prikaz.ActualWidth,
					Canvas_Prikaz.ActualHeight,
					tocka,
					//	FLAG_bPerspektivnaKameraAktivna
					false
					);

			return p2dKanvasTrenutnaKoordinataMisa;

		}
		private Point kanvasTrenutnaKoordinataMisaPersp()
		{
			Point p2dKanvasTrenutnaKoordinataMisa = rotKam.Projektor_2DKanvasViaKanvasKursorPoz(
					Canvas_Prikaz.ActualWidth,
					Canvas_Prikaz.ActualHeight,
					System.Windows.Input.Mouse.GetPosition(Canvas_Prikaz),
					FLAG_KameraPerspektivnaAktivna
					);

			return p2dKanvasTrenutnaKoordinataMisa;

		}
		private Point kanvasTrenutnaKoordinataMisaPersp(Point tocka)
		{
			Point p2dKanvasTrenutnaKoordinataMisa = rotKam.Projektor_2DKanvasViaKanvasKursorPoz(
					Canvas_Prikaz.ActualWidth,
					Canvas_Prikaz.ActualHeight,
					tocka,
					FLAG_KameraPerspektivnaAktivna
					);

			return p2dKanvasTrenutnaKoordinataMisa;

		}


		private static Model3DGroup citajIzXamla(string path)
		{
			// funkcija za testiranje

			System.Windows.Markup.XamlReader xr = new System.Windows.Markup.XamlReader();
			FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
			object tmp = xr.LoadAsync(fs);
			fs.Close();

			Console.WriteLine(tmp.ToString());

			Model3DGroup mg = null;
			if (tmp.GetType().ToString().EndsWith("Viewport3D"))
			{
				Viewport3D vp3d = (Viewport3D)tmp;
				ModelVisual3D mv3d = (ModelVisual3D)vp3d.Children[0];
				mg = (Model3DGroup)mv3d.Content;
			}
			else if (tmp.GetType().ToString().EndsWith("Model3DGroup"))
			{
				mg = (Model3DGroup)tmp;
			}
			else if (tmp.GetType().ToString().EndsWith("Page"))
			{
				Page p = (Page)tmp;

			}


			return mg;
			//modelGroup1.Children.Add(mg);
		}
		private static ModelVisual3D citajIzXamla_v3d(string path)
		{
			// funkcija za testiranje

			System.Windows.Markup.XamlReader xr = new System.Windows.Markup.XamlReader();
			FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
			object tmp = xr.LoadAsync(fs);
			fs.Close();

			//	Console.WriteLine(tmp.ToString());

			if (tmp.GetType().ToString().EndsWith("Viewport3D"))
			{
				Viewport3D vp3d = (Viewport3D)tmp;
				ModelVisual3D mv3 = ((ModelVisual3D)vp3d.Children[0]);
				vp3d.Children.Clear();
				return mv3;
			}
			else { return null; }

		}


		// test tipke
		/*
		private void button1_Click(object sender, RoutedEventArgs e)
		{
			ModelVisual3D mv = new ModelVisual3D();
			mv.Content = mojModel.MODEL_minki.Clone();
			//mv.Content = citajIzXamla("belakocka.xaml");
			Viewport3D_Prikaz.Children.Add(mv);

		}
		*/
		// animacija preko frameova
		/*
		private void kamAnimiraj(RotKam.KamFrame f)
		{
			if (f != null)
			{
				Duration dur = new Duration(TimeSpan.FromSeconds(1));

				Point3D kamRotKut_trenutno = rotrot.KameraRotacijaKuteviXYZ_dp;
				Point3D kamRotKut_frame = f.KameraRotacijaKuteviZXY;
				Point3D kamRotKut_najkraci = f.KameraRotacijaKuteviZXY;

				// ako treba napraviti rotaciju za vise od pola kruga vrti u suprotnom smjeru
				double z = kamRotKut_frame.Z-kamRotKut_trenutno.Z;
				if (z > 180) { kamRotKut_najkraci.Z = kamRotKut_frame.Z - 360; }
				else if (z < -180) { kamRotKut_najkraci.Z = kamRotKut_frame.Z + 360; }
				
				double x = kamRotKut_frame.X - kamRotKut_trenutno.X;
				if (x > 180) { kamRotKut_najkraci.X = kamRotKut_frame.X - 360; }
				else if (x < -180) { kamRotKut_najkraci.X = kamRotKut_frame.X + 360; }

				double y = kamRotKut_frame.Y - kamRotKut_trenutno.Y;
				if (y > 180) { kamRotKut_najkraci.Y = kamRotKut_frame.Y - 360; }
				else if (y < -180) { kamRotKut_najkraci.Y = kamRotKut_frame.Y + 360; }


				Point3DAnimation anim_rot = new Point3DAnimation(kamRotKut_najkraci, dur, FillBehavior.HoldEnd);
				Vector3DAnimation anim_trans = new Vector3DAnimation(f.KameraTranslacija, dur, FillBehavior.HoldEnd);
				DoubleAnimation anim_zoom = new DoubleAnimation(f.KameraZoom, dur, FillBehavior.HoldEnd);

				// buduci da imaju isti duration dovoljno je da jedna animacija digne event
				anim_zoom.Completed += new EventHandler(anim_Completed);

				FLAG_bKameraAnimating = true;
				rotrot.BeginAnimation(RotKam.KameraTranslacijaProperty, anim_trans);
				rotrot.BeginAnimation(RotKam.KameraRotacijaKuteviXYZProperty, anim_rot);
				rotrot.BeginAnimation(RotKam.KameraZoom_dpProperty, anim_zoom);
			}
		}
		private void anim_Completed(object sender, EventArgs e)
		{
			// sacuvaj transformaciju animacije kao lokalnu vrijednost
			rotrot.KameraTranslacija_dp = rotrot.KameraTranslacija_dp;
			rotrot.KameraRotacijaKuteviXYZ_dp = rotrot.KameraRotacijaKuteviXYZ_dp;
			rotrot.KameraZoom_dp = rotrot.KameraZoom_dp;

			// makni animaciju da se opet mogu mijenjati lokalne vrijednosti
			rotrot.BeginAnimation(RotKam.KameraTranslacijaProperty, null);
			rotrot.BeginAnimation(RotKam.KameraRotacijaKuteviXYZProperty, null);
			rotrot.BeginAnimation(RotKam.KameraZoom_dpProperty, null);

			FLAG_bKameraAnimating = false;
		}
		*/

		// ako se pozicija nije promijenila preko slidera
		private void rotrot_EVENT_PromjenaParametaraKamere(object sender, EventArgs e)
		{
			modelSelectedKruzicUpdate();

			// svjetlo ide iz kamere
			FLAG_DirLightKamera.Direction = -rotKam.KameraRotacijaVektor;

			double x = rotKam.KameraRotacijaKuteviXYZ_dp.X;
			//double y = rotrot.KameraRotacijaKuteviZXY_dp.Y;
			double z = rotKam.KameraRotacijaKuteviXYZ_dp.Z;
			/*
			if (!FLAG_bKameraAnimating)
			{
				ListBox_cameraFrame.SelectedIndex = -1;
			}
			*/

			// sakrij osi ovisno o pravim kutevima
			/*
			if ((x % 180 == 0) && (z % 180 == 0)) { xyzOsiVidljive(true, false, true); }
			else if ((x % 180 == 0) && (Math.Abs(z) == 90)) { xyzOsiVidljive(false, true, true); }
			else if (Math.Abs(x) == 90) { xyzOsiVidljive(true, true, false); }
			else { xyzOsiVidljive(true, true, true); }
			*/

		}
		// promjena slidera za rot, translaciju i zum
		/*
		private void slider_cameraXYZRotate_ValueChanged(object sender, RoutedEventArgs e)
		{
			if (!FLAG_bNamjestamVrijednostiUKontroli)
			{
				rotrot.KameraRotacijaKuteviXYZ_dp = new Point3D(
					slider_cameraBeta.Value,
					slider_cameraGama.Value,
					slider_cameraAlfa.Value);
			}
		}
		private void slider_canvasXYZtranslate(object sender, RoutedEventArgs e)
		{
			if (!FLAG_bNamjestamVrijednostiUKontroli)
			{
				rotrot.KameraTranslacija_dp =
					new Vector3D(
						slider_canvasXtranslate.Value,
						slider_canvasYtranslate.Value,
						slider_canvasZtranslate.Value);
			}
		}
		private void slider_cameraZoom_ValueChanged(object sender, RoutedEventArgs e)
		{
			if (!FLAG_bNamjestamVrijednostiUKontroli)
			{
				{
					rotrot.KameraZoom_dp = slider_cameraZoom.Value;
				}
			}
		}

		
		private void kamParametreUSlidere()
		{
			// ako slider nije micao kameru
			FLAG_bNamjestamVrijednostiUKontroli = true;

			slider_cameraZoom.Value = rotrot.KameraZoom_dp;
			slider_cameraAlfa.Value = rotrot.KameraRotacijaKuteviXYZ_dp.Z;
			slider_cameraBeta.Value = rotrot.KameraRotacijaKuteviXYZ_dp.X;
			slider_cameraGama.Value = rotrot.KameraRotacijaKuteviXYZ_dp.Y;
			slider_canvasXtranslate.Value = rotrot.KameraTranslacija_dp.X;
			slider_canvasYtranslate.Value = rotrot.KameraTranslacija_dp.Y;
			slider_canvasZtranslate.Value = rotrot.KameraTranslacija_dp.Z;

			FLAG_bNamjestamVrijednostiUKontroli = false;

		}
		*/

		// tipke predefiniranih pozicija
		/*
		private void Button_cameraOrtPer_Click(object sender, RoutedEventArgs e)
		{
			if (Viewport3D_Prikaz.Camera.GetType() == typeof(OrthographicCamera))
			{
				Viewport3D_Prikaz.Camera = rotrot.KameraPer;
				FLAG_bPerspektivnaKameraAktivna = true;
			}
			else
			{
				Viewport3D_Prikaz.Camera = rotrot.KameraOrt;
				FLAG_bPerspektivnaKameraAktivna = false;
			}
			modelSelectedKruzicUpdate();
		}
		private void button_nacrt_Click(object sender, RoutedEventArgs e)
		{
			rotrot.KamPredefiniraneRotacije_Nacrt();
		}
		private void button_straga_Click(object sender, RoutedEventArgs e)
		{
			rotrot.KamPredefiniraneRotacije_Straga();
		}
		private void button_tlocrt_Click(object sender, RoutedEventArgs e)
		{
			rotrot.KamPredefiniraneRotacije_Tlocrt();
		}
		private void button_odozdo_Click(object sender, RoutedEventArgs e)
		{
			rotrot.KamPredefiniraneRotacije_Odozdo();
		}
		private void button_bokocrt_Click(object sender, RoutedEventArgs e)
		{
			rotrot.KamPredefiniraneRotacije_Bokocrt();
		}
		private void button_lijevo_Click(object sender, RoutedEventArgs e)
		{
			rotrot.KamPredefiniraneRotacije_Slijeva();
		}
		private void button_perspektiva_Click(object sender, RoutedEventArgs e)
		{
			rotrot.KamPredefiniraneRotacije_Perspektiva();
		}
		*/

		// listbox za frameove
		/*
		private void Button_cameraFrameDodaj_Click(object sender, RoutedEventArgs e)
		{
			Rota.KamFrame fr = new Rota.KamFrame(
				"Frame " + (ListBox_cameraFrame.Items.Count + 1),
				rotrot.KameraTranslacija_dp,
				rotrot.KameraRotacijaKuteviXYZ_dp,
				rotrot.KameraZoom_dp);

			ListBox_cameraFrame.Items.Add(fr);

		}
		private void Button_cameraFrameMakni_Click(object sender, RoutedEventArgs e)
		{
			int index = ListBox_cameraFrame.SelectedIndex;
			if (index > -1)
			{
				ListBox_cameraFrame.Items.RemoveAt(index);

				ListBox_cameraFrame.SelectedIndex = index;
			}
		}
		private void ListBox_cameraFrame_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			kamAnimiraj((Rota.KamFrame)ListBox_cameraFrame.SelectedItem);
		}
		*/

		public void _REFRESH()
		{
			modelSelectedKruzicUpdate();
		}
		public void _KAVNAS_VISIBLE(bool vidljiv)
		{
			if (vidljiv)
			{
				Canvas_Prikaz.IsHitTestVisible = true;
				//Canvas_Prikaz.Visibility = Visibility.Visible;
			}
			else
			{
				Canvas_Prikaz.IsHitTestVisible = false;
				//Canvas_Prikaz.Visibility = Visibility.Hidden;
			}
		}

		// kontrola objekata
		public void _MOUSE_DOWN(System.Windows.Input.MouseButtonEventArgs e)
		{

			//Viewport3D_Prikaz.MouseMove
			FLAG_p2dKanvasKoordStara = kanvasTrenutnaKoordinataMisaOrto();

			switch (e.ChangedButton)
			{
				case System.Windows.Input.MouseButton.Left:
					// potvrdi transformaciju objekta
					if (FLAG_TransformObjektAktiv)
					{
						modelSelectedTransformEnd();
					}
					else if (FLAG_SelektBoxAktiv)
					{
						selektBoxApply();
					}
					// zapocni box select
					else
					{
						selektBoxBegin();
					}

					break;
				case System.Windows.Input.MouseButton.Middle:
					FLAG_KameraOrbit = true;
					break;
				case System.Windows.Input.MouseButton.Right:
					FLAG_KameraPan = true;
					break;
			}
		}
		public void _MOUSE_UP(System.Windows.Input.MouseButtonEventArgs e)
		{
			switch (e.ChangedButton)
			{
				case System.Windows.Input.MouseButton.Left:
					if (FLAG_SelektBoxAktiv)
					{
						selektBoxApply();
					}
					break;
				case System.Windows.Input.MouseButton.Middle:
					FLAG_KameraOrbit = false;
					break;
				case System.Windows.Input.MouseButton.Right:
					FLAG_KameraPan = false;
					break;
			}
		}

		public void _MOUSE_MOVE(Point _MousePozStara, Point _MousePozNova)
		{
			// koordinata se mijenja ovisno o zoomu
			Point p2dKanvasTrenutnaKoordinataMisa = kanvasTrenutnaKoordinataMisaOrto(_MousePozNova);
			FLAG_p2dKanvasKoordStara = kanvasTrenutnaKoordinataMisaOrto(_MousePozStara);
			/*
			if (FLAG_bPerspektivnaKameraAktivna)
			{
				Point p2dKanvasTrenutnaKoordinataMisaPer = kanvasTrenutnaKoordinataMisaPersp();

				label_status_koord.Content = string.Format("Persp, X: {0}  Y: {1}",
						p2dKanvasTrenutnaKoordinataMisaPer.X.ToString("0.000"),
						p2dKanvasTrenutnaKoordinataMisaPer.Y.ToString("0.000"));
			}
			else
			{
				label_status_koord.Content = string.Format("Orto, X: {0}  Y: {1}",
						p2dKanvasTrenutnaKoordinataMisa.X.ToString("0.000"),
						p2dKanvasTrenutnaKoordinataMisa.Y.ToString("0.000"));
			}
			*/
			Vector p2dDelta = (p2dKanvasTrenutnaKoordinataMisa - FLAG_p2dKanvasKoordStara);

			if (FLAG_SelektBoxAktiv)
			{
				Point p2dOrigin = FLAG_p2dSelektBoxKanvasMousePozOrigin;
				Point p2dTrenutno = System.Windows.Input.Mouse.GetPosition(Canvas_Prikaz);

				double x = (p2dOrigin.X < p2dTrenutno.X) ? p2dOrigin.X : p2dTrenutno.X;
				double y = (p2dOrigin.Y < p2dTrenutno.Y) ? p2dOrigin.Y : p2dTrenutno.Y;
				double w = Math.Abs(p2dOrigin.X - p2dTrenutno.X);
				double h = Math.Abs(p2dOrigin.Y - p2dTrenutno.Y);

				FLAG_rectBoxSelect.Margin = new Thickness(x, y, 0, 0);
				FLAG_rectBoxSelect.Width = w + 1;
				FLAG_rectBoxSelect.Height = h + 1;
			}
			else if (FLAG_KameraOrbit)
			{
				double brzinaRotacijeKoef = 300; // sporije...100, 200, 300...brze

				// bez obzira na zoom brzina rotacije ostaje ista
				double kutOkoZDodatni = p2dDelta.X / rotKam.KameraZoom_dp * brzinaRotacijeKoef;
				double kutOkoXDodatni = p2dDelta.Y / rotKam.KameraZoom_dp * brzinaRotacijeKoef;


				/*
				Point3D kutDodatni = (rotKam.KameraPozicijaUpVektor.Z >= 0) ?
					new Point3D(kutOkoXDodatni, 0, -kutOkoZDodatni)
					: new Point3D(kutOkoXDodatni, 0, kutOkoZDodatni);
				*/

				Point3D kutDodatni = new Point3D(kutOkoXDodatni, 0, -kutOkoZDodatni);
				rotKam.KamRotateRelativ(kutDodatni);


				/*
				Point3D kutDodatni = new Point3D(kutOkoXDodatni, 0, -kutOkoZDodatni);

				Vector3D v = (Vector3D)kutDodatni;
				Rota.prostorneRotacije.Z(kutDodatni, rotrot.KameraRotacijaVektor.Y);

				rotrot.KamRotateRelativ((Point3D)v);
				*/
			}
			else if (FLAG_KameraPan)
			{
				double brzinaTranslacijeKoef = 1; // 1 prati kursor, manje=sporije, vece=brze

				rotKam.KamTranslateRelativ(rotKam.Projektor_v3dTranslateViaKanvasKoord(-p2dDelta * brzinaTranslacijeKoef));
			}
			else if (FLAG_KameraZoom)
			{

				//double deltaRadijus = ((Vector)p2dKanvasTrenutnaKoordinataMisa).Length - ((Vector)FLAG_p2dKanvasKoordStara).Length;
				//rotrot.KamZoomRelativ(deltaRadijus);
				double brzinaZoomKoef = 3;
				double deltaZoom = p2dKanvasTrenutnaKoordinataMisa.Y - FLAG_p2dKanvasKoordStara.Y;
				rotKam.KamZoomRelativ(-deltaZoom * brzinaZoomKoef);
			}


			// vazan je redoslijed elseif-ova, kamera ima veci prioritet

			// transformacije objekta
			else if (FLAG_SelektPostoji)
			{
				//move
				if (FLAG_TransformObjektMove)
				{
					Vector3D ofset = rotKam.Projektor_v3dTranslateViaKanvasKoord(p2dDelta);

					if (FLAG_TransformConstrainX)
					{
						ofset.Y = 0;
						ofset.Z = 0;
					}
					else if (FLAG_TransformConstrainY)
					{
						ofset.X = 0;
						ofset.Z = 0;
					}
					else if (FLAG_TransformConstrainZ)
					{
						ofset.Y = 0;
						ofset.X = 0;
					}

					foreach (Selekt sel in FLAG_SelKolekcija)
					{
						ModelVisual3D _mv3 = sel.mv3_Model;

						Matrix3D mx3 = _mv3.Transform.Value;
						mx3.Translate(ofset);
						MatrixTransform3D mt3 = new MatrixTransform3D(mx3);
						_mv3.Transform = mt3;
					}
					modelSelectedKruzicUpdate();
				}
				//rotate
				else if (FLAG_TransformObjektRotate)
				{
					Point p2dSelektGrupaCentarKanvas2D = rotKam.Projektor_2DKanvasVia3DViewport(FLAG_p3dGrupSelektCentar, FLAG_KameraPerspektivnaAktivna);

					Vector v2dStari = (FLAG_p2dKanvasKoordStara - p2dSelektGrupaCentarKanvas2D);
					Vector v2dNovi = (p2dKanvasTrenutnaKoordinataMisa - p2dSelektGrupaCentarKanvas2D);
					double kut = Vector.AngleBetween(v2dStari, v2dNovi);

					Quaternion q = Quaternion.Identity;
					if (FLAG_TransformConstrainX)
					{
						q = new Quaternion(new Vector3D(1, 0, 0), kut);
					}
					else if (FLAG_TransformConstrainY)
					{
						q = new Quaternion(new Vector3D(0, -1, 0), kut);
					}
					else if (FLAG_TransformConstrainZ)
					{
						q = new Quaternion(new Vector3D(0, 0, 1), kut);
					}
					else
					{
						q = new Quaternion(rotKam.KameraRotacijaVektor, kut);
					}

					foreach (Selekt sel in FLAG_SelKolekcija)
					{
						ModelVisual3D _mv3 = sel.mv3_Model;

						Matrix3D mx3 = _mv3.Transform.Value;
						mx3.RotateAt(
							q,
							FLAG_p3dGrupSelektCentar);

						MatrixTransform3D mt3 = new MatrixTransform3D(mx3);
						_mv3.Transform = mt3;
					}
					modelSelectedKruzicUpdate();
				}
				//scale
				else if (FLAG_TransformObjektScale)
				{
					Point p2dSelektGrupaCentarKanvas2D = rotKam.Projektor_2DKanvasVia3DViewport(FLAG_p3dGrupSelektCentar, FLAG_KameraPerspektivnaAktivna);

					//Point p2dObjSredisteKanvas2D = rotrot.Koord2dKanvasViaViewportPoint3d(new Point3D(mx3.OffsetX, mx3.OffsetY, mx3.OffsetZ), FLAG_bPerspektivnaKameraAktivna);
					Vector v2dScaleKruznicaRadijusStara = FLAG_p2dKanvasKoordStara - p2dSelektGrupaCentarKanvas2D;
					Vector v2dScaleKruznicaRadijusTrenutna = p2dKanvasTrenutnaKoordinataMisa - p2dSelektGrupaCentarKanvas2D;

					double dScaleKoef = 1;
					double dScaleKruznicaRadijusStara = v2dScaleKruznicaRadijusStara.Length;
					double dScaleKruznicaRadijusTrenutna = v2dScaleKruznicaRadijusTrenutna.Length;

					// da ne bi doslo do dijeljenja s nulom i beskonacnosti
					if (dScaleKruznicaRadijusStara < 0.001) { dScaleKruznicaRadijusStara = 0.001; }
					if (dScaleKruznicaRadijusTrenutna < 0.001) { dScaleKruznicaRadijusTrenutna = 0.001; }
					dScaleKoef = dScaleKruznicaRadijusTrenutna / dScaleKruznicaRadijusStara;

					// na prijelazu kursora izmedju lijeve i desne strane centra obrni model
					if (((v2dScaleKruznicaRadijusStara.X >= 0) && (v2dScaleKruznicaRadijusTrenutna.X < 0))
						|| ((v2dScaleKruznicaRadijusStara.X < 0) && (v2dScaleKruznicaRadijusTrenutna.X >= 0)))
					{
						dScaleKoef = -dScaleKoef;
					}


					/*
					if (v2dScaleKruznicaRadijusTrenutna.X < 0)
					{
						dScaleKoef = -dScaleKoef;
						Console.WriteLine(dScaleKoef);
					}
					 */


					Vector3D scale = new Vector3D(1, 1, 1);
					if (FLAG_TransformConstrainX)
					{
						scale = new Vector3D(dScaleKoef, 1, 1);
					}
					else if (FLAG_TransformConstrainY)
					{
						scale = new Vector3D(1, dScaleKoef, 1);
					}
					else if (FLAG_TransformConstrainZ)
					{
						scale = new Vector3D(1, 1, dScaleKoef);
					}
					else
					{
						scale = new Vector3D(dScaleKoef, dScaleKoef, dScaleKoef);
					}

					foreach (Selekt sel in FLAG_SelKolekcija)
					{
						ModelVisual3D _mv3 = sel.mv3_Model;

						Matrix3D mx3 = _mv3.Transform.Value;
						mx3.ScaleAt(
							scale,
							FLAG_p3dGrupSelektCentar
							);
						MatrixTransform3D mt3 = new MatrixTransform3D(mx3);
						_mv3.Transform = mt3;
					}
					modelSelectedKruzicUpdate();
				}
			}

			FLAG_p2dKanvasKoordStara = p2dKanvasTrenutnaKoordinataMisa;

			/*
			Console.WriteLine();
			Console.WriteLine(FLAG_kanvasMouseLeftClickHold);
			Console.WriteLine(FLAG_kanvasMouseMiddleClickHold);
			Console.WriteLine(FLAG_kanvasMouseRightClickHold);
			 */
		}
		public void _KEY_DOWN(System.Windows.Input.KeyEventArgs e)
		{
			//MessageBox.Show(e.Key.ToString());

			if (e.Key == Key.Space)
			{
				if (FLAG_SelKolekcija.Count == 1)
				{
					Selekt sel = (Selekt)FLAG_SelKolekcija[0];
					ModelVisual3D _mv3 = sel.mv3_Model;

					MatricaRotacije matRot = new MatricaRotacije(_mv3.Transform.Value);

					Vector3D v3dProbni = matRot * new Vector3D(0, -1, 0);
					Vector3D v3dProbni_Up = matRot * new Vector3D(0, 0, 1);
					//v3d_probni.Normalize();

					double okoZ = 0;
					double okoX = 0;
					double okoY = 0;
					double XYProj = Math.Sqrt(v3dProbni.X * v3dProbni.X + v3dProbni.Y * v3dProbni.Y);

					Console.WriteLine(
						//Vector3D.AngleBetween(new Vector3D(1, 0, 0), new Vector3D(-1, 0, 0))

						);
					Console.WriteLine("kam: " + rotKam.KameraPozicijaUpVektor);
					Console.WriteLine("prob: " + v3dProbni_Up);

					okoY = Vector3D.AngleBetween(
						rotKam.KameraPozicijaUpVektor,
						v3dProbni_Up);
					Console.WriteLine("okoY: " + okoY);

					//if (v3dProbni_Up.Z < 0) { okoY = -okoY; }

					// os Z
					if (v3dProbni.X >= 0)
					{
						okoZ = Math.Acos(-v3dProbni.Y / XYProj);
					}
					else
					{
						okoZ = -Math.Acos(-v3dProbni.Y / XYProj);
					}
					// os X
					okoX = -Math.Atan(v3dProbni.Z / XYProj);


					//okoX = -Math.Acos(XYProj / v3d_probni.Length);

					/*
					okoX = -Math.Atan(v3d_probni.Z / XYProj);
					if (v3dUp_probni.Z < 0)
					{
						if (v3d_probni.Z > 0)
						{
							okoX += Math.PI;
						}
						else if (v3d_probni.Z < 0)
						{
							okoX -= Math.PI;
						}
						
					}

					*/
					okoZ = prostorneRotacije.radijani_u_stupnjeve(okoZ);
					okoX = prostorneRotacije.radijani_u_stupnjeve(okoX);


					//	Console.WriteLine(v3d_probni);
					//	Console.WriteLine(okoZ);
					//	Console.WriteLine(okoX);


					Point3D p3dObjSrediste3D = new Point3D(_mv3.Transform.Value.OffsetX, _mv3.Transform.Value.OffsetY, _mv3.Transform.Value.OffsetZ);

					KamFrame kf = new KamFrame(
						(Vector3D)p3dObjSrediste3D,
						new Point3D(okoX, 0, okoZ),
						_mv3.Content.Bounds.SizeX
						);
					if (okoX != double.NaN && okoZ != double.NaN)
					{
						rotKam.KamAnimiraj(kf, 500);

						/*
						rotKam.KameraZoom_dp = _mv3.Content.Bounds.SizeX;
						rotKam.KameraTranslacija_dp = (Vector3D)p3dObjSrediste3D;
						rotKam.KameraRotacijaKuteviXYZ_dp = FLAG_ObjectVisualRotation;
						//rotKam.KameraRotacijaKuteviXYZ_dp = new Point3D(okoX, 0, okoZ);
						*/
					}
				}
			}



			// emulacija  tipke misa za laptop
			switch (e.Key)
			{
				case Key.LeftShift:
					FLAG_KameraOrbit = true;
					break;
				/*
			case Key.LeftCtrl:
				FLAG_KameraPan = true;
				break;
				*/
				case Key.LeftCtrl:
				case Key.LeftAlt:
					FLAG_KameraZoom = true;
					break;
			}

			Point p2dKanvasTrenutnaKoordinataMisaOrto = kanvasTrenutnaKoordinataMisaOrto();
			FLAG_p2dKanvasKoordStara = p2dKanvasTrenutnaKoordinataMisaOrto;

			// box selekt
			if (e.Key == Key.B)
			{
				if (FLAG_SelektBoxAktiv)
				{
					selektBoxApply();
				}
				else
				{
					if (!FLAG_TransformObjektAktiv)
					{
						selektBoxBegin();
					}
				}
			}
			// transform
			else if (FLAG_SelektPostoji)
			{
				//FLAG_p2dTransformKanvasKoordOrigin = p2dKanvasTrenutnaKoordinataMisaOrto;

				if (FLAG_TransformObjektAktiv)
				{
					switch (e.Key)
					{
						case System.Windows.Input.Key.X:
							FLAG_TransformConstrainX = !FLAG_TransformConstrainX;
							FLAG_TransformConstrainY = false;
							FLAG_TransformConstrainZ = false;
							break;
						case System.Windows.Input.Key.Y:
							FLAG_TransformConstrainX = false;
							FLAG_TransformConstrainY = !FLAG_TransformConstrainY;
							FLAG_TransformConstrainZ = false;
							break;
						case System.Windows.Input.Key.Z:
							FLAG_TransformConstrainX = false;
							FLAG_TransformConstrainY = false;
							FLAG_TransformConstrainZ = !FLAG_TransformConstrainZ;
							break;
					}
				}

				switch (e.Key)
				{
					case System.Windows.Input.Key.M:
						if (FLAG_TransformObjektAktiv && !FLAG_TransformObjektMove)
						{
							modelSelectedTransformCancel();
						}
						else if (FLAG_TransformObjektMove)
						{
							modelSelectedTransformEnd();
						}
						else
						{
							FLAG_TransformObjektMove = true;
							modelSelectedTransformBegin();
						}
						break;
					case System.Windows.Input.Key.R:
						if (FLAG_TransformObjektAktiv && !FLAG_TransformObjektRotate)
						{
							modelSelectedTransformCancel();
						}
						else if (FLAG_TransformObjektRotate)
						{
							modelSelectedTransformEnd();
						}
						else
						{
							FLAG_TransformObjektRotate = true;
							modelSelectedTransformBegin();
						}
						break;
					case System.Windows.Input.Key.S:
						if (FLAG_TransformObjektAktiv && !FLAG_TransformObjektScale)
						{
							modelSelectedTransformCancel();
						}
						else if (FLAG_TransformObjektScale)
						{
							modelSelectedTransformEnd();
						}
						else
						{
							FLAG_TransformObjektScale = true;
							modelSelectedTransformBegin();
						}

						break;

					case System.Windows.Input.Key.Enter:
						if (FLAG_TransformObjektAktiv)
						{
							modelSelectedTransformEnd();
						}
						break;
					case System.Windows.Input.Key.Escape:
						if (FLAG_TransformObjektAktiv)
						{
							modelSelectedTransformCancel();
						}
						break;
					case System.Windows.Input.Key.Delete:

						//obrisi
						foreach (Selekt sel in FLAG_SelKolekcija)
						{
							Viewport3D_Prikaz.Children.Remove(sel.mv3_Model);
						}

						// makni selekciju
						modelSelectedMakniSve();
						break;
				}
			}

			switch (e.Key)
			{
				// add object
				case System.Windows.Input.Key.A:

					// jedini koristi FLAG_perspektivnaKameraAktivna
					// zbog stavljanja objekta u perspektivi
					p2dKanvasTrenutnaKoordinataMisaOrto = rotKam.Projektor_2DKanvasViaKanvasKursorPoz(
							Canvas_Prikaz.ActualWidth,
							Canvas_Prikaz.ActualHeight,
							System.Windows.Input.Mouse.GetPosition(Canvas_Prikaz),
							FLAG_KameraPerspektivnaAktivna
							);

					Point3D p3d = rotKam.Projektor_3DViewportVia2DKanvas(p2dKanvasTrenutnaKoordinataMisaOrto);
					// ako treba stavljat obj na fiksnu udaljenost od kamere
					//Point3D p3d = rotrot.Koord3dViewportViaKanvas2dKoord(p2dKanvasTrenutnaKoordinataMisaOrto, 10);

					ModelVisual3D mv3 = new ModelVisual3D();
					//mv3.Content = mojModel.MODEL_kockica.Clone();
					mv3.Content = mojModel.MODEL_xyz1.Clone();

					//mv3 = citajIzXamla_v3d("kockaPuc.xaml");

					Matrix3D mx3 = Matrix3D.Identity;


					Quaternion qz = new Quaternion(new Vector3D(0, 0, 1), rotKam.KameraRotacijaKuteviXYZ_dp.Z);
					Quaternion qx = new Quaternion(new Vector3D(1, 0, 0), rotKam.KameraRotacijaKuteviXYZ_dp.X);
					Quaternion qy = new Quaternion(new Vector3D(0, 1, 0), rotKam.KameraRotacijaKuteviXYZ_dp.Y);
					Quaternion qzxy = qz * qx * qy;

					mx3.Rotate(qzxy);
					mx3.Translate((Vector3D)p3d);

					MatrixTransform3D mt3 = new MatrixTransform3D(mx3);
					mv3.Transform = mt3;

					Viewport3D_Prikaz.Children.Add(mv3);
					//	FLAG_SelKolekcija[0] = mv3;
					// modelGroupGlavni.Children.Add(Modeltmp);

					break;
				// kamera orto persp
				case System.Windows.Input.Key.C:
					if (Viewport3D_Prikaz.Camera.GetType() == typeof(OrthographicCamera))
					{
						Viewport3D_Prikaz.Camera = rotKam.KameraPer;
						FLAG_KameraPerspektivnaAktivna = true;
					}
					else
					{
						Viewport3D_Prikaz.Camera = rotKam.KameraOrt;
						FLAG_KameraPerspektivnaAktivna = false;
					}
					modelSelectedKruzicUpdate();
					break;
			}
		}
		public void _KEY_UP(System.Windows.Input.KeyEventArgs e)

		{
			switch (e.Key)
			{
				case Key.LeftShift:
					FLAG_KameraOrbit = false;
					break;
				/*
			case Key.LeftCtrl:
				FLAG_KameraPan = false;
				break;
				*/
				case Key.LeftCtrl:
				case Key.LeftAlt:
					FLAG_KameraZoom = false;
					break;
			}
		}

		public void _MOUSE_WHEEL(System.Windows.Input.MouseWheelEventArgs e)
		{
			FLAG_p2dKanvasKoordStara = kanvasTrenutnaKoordinataMisaOrto();

			double delta = e.Delta / 120;
			double brzinaZoomaKoef = rotKam.KameraZoom_dp / 5;
			//zoomiranje
			rotKam.KamZoomRelativ(-delta * brzinaZoomaKoef);

			//Console.WriteLine(e.Delta);

			/*
			if (System.Windows.Input.Mouse.RightButton == MouseButtonState.Pressed)
			{
				// translacija kamere od centra u koji gleda
				rotrot.KamTranslateRelativ(rotrot.Projektor_v3dTranslateViaKanvasKoord(
					new Vector3D(0, 0, delta)));
			}
			else
			{
				double brzinaZoomaKoef = rotrot.KameraZoom_dp / 5;
				//zoomiranje
				rotrot.KamZoomRelativ(-delta * brzinaZoomaKoef);
			}
			*/

		}

		private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			modelSelectedKruzicUpdate();
		}
	}
}