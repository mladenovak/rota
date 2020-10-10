using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Media.Animation;

namespace Rota
{
	

	public class RotaKamera : Animatable
	{
		protected override Freezable CreateInstanceCore()
		{
			throw new NotImplementedException();
		}

		// dp sluze za animaciju kamere
		public static readonly DependencyProperty KameraTranslacijaProperty =
			DependencyProperty.Register("KameraTranslacija_dp", typeof(Vector3D), typeof(RotaKamera), new UIPropertyMetadata(new Vector3D(0, 0, 0), RotaKamera.HandlePropertyChange));
		// radi lakse animacije kutevi rotacije okoZ, okoX, okoY su stavljani redom u Point3D.Z,Point3D.X,Point3D.Y
		// zeto jer postoji Point3DAnimation koji ce onda mijenjati sva 3 kuta istovremeno
		public static readonly DependencyProperty KameraRotacijaKuteviXYZProperty =
			DependencyProperty.Register("KameraRotacijaKuteviXYZ_dp", typeof(Point3D), typeof(RotaKamera), new UIPropertyMetadata(new Point3D(0, 0, 0), RotaKamera.HandlePropertyChangeKuteviZXY, RotaKamera.CoerceRotateXYZ));
		public static readonly DependencyProperty KameraZoom_dpProperty =
			DependencyProperty.Register("KameraZoom_dp", typeof(double), typeof(RotaKamera), new UIPropertyMetadata((double)1, RotaKamera.HandlePropertyChange, RotaKamera.CoerceZoom));

		private static object CoerceRotateXYZ(DependencyObject d, object value)
		{
			Point3D xyzStupnjevi = (Point3D)value;

			// da kutevi ne bi bili preveliki drzi ih unutar +-180°
			while (xyzStupnjevi.Z > 180) { xyzStupnjevi.Z -= 360; }
			while (xyzStupnjevi.Z < -180) { xyzStupnjevi.Z += 360; }
			while (xyzStupnjevi.X > 180) { xyzStupnjevi.X -= 360; }
			while (xyzStupnjevi.X < -180) { xyzStupnjevi.X += 360; }
			while (xyzStupnjevi.Y > 180) { xyzStupnjevi.Y -= 360; }
			while (xyzStupnjevi.Y < -180) { xyzStupnjevi.Y += 360; }

			return xyzStupnjevi;
		}
		private static object CoerceZoom(DependencyObject d, object value)
		{
			// zoom unutar granica
			double zoomMin = 0.5;
			double zoomMax = 50;
			double zoom = (double)value;

			//return zoom;

			if (zoom < zoomMin) { zoom = zoomMin; }
			else if (zoom > zoomMax) { zoom = zoomMax; }
			
			return zoom;
		}
		
		private static void HandlePropertyChange(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			((RotaKamera)source).KamNamjestiIzGlobalnihParametara();
		}
		private static void HandlePropertyChangeKuteviZXY(DependencyObject source, DependencyPropertyChangedEventArgs e)
		{
			// kad se promijene kutevi mora nanovo izracunati KameraRotacijaVektor i KamUp
			((RotaKamera)source).KamRotateApsolut(((RotaKamera)source).KameraRotacijaKuteviXYZ_dp.Z, ((RotaKamera)source).KameraRotacijaKuteviXYZ_dp.X,((RotaKamera)source).KameraRotacijaKuteviXYZ_dp.Y);
			((RotaKamera)source).KamNamjestiIzGlobalnihParametara();
		}

		// public properties
		/// <summary>
		/// Apsolutna translacija kamere (od pozicije na sferi polumjera 1).
		/// </summary>
		public Vector3D KameraTranslacija_dp
		{
			get { return (Vector3D)GetValue(KameraTranslacijaProperty); }
			set { SetValue(KameraTranslacijaProperty, value); }
		}
		/// <summary>
		/// Kutevi apsolutne rotacije oko osi X, Y, Z po sferi polumjera 1.
		/// </summary>
		public Point3D KameraRotacijaKuteviXYZ_dp
		{
			get { return (Point3D)GetValue(KameraRotacijaKuteviXYZProperty); }
			set { SetValue(KameraRotacijaKuteviXYZProperty, value); }
		}
		/// <summary>
		/// Vrijednost zooma. 1 - zoom nije faktor; vece od 1 - kamera se udaljava od tocke fokusa za taj faktor; manje od 1 - kamera se priblizava.
		/// </summary>
		public double KameraZoom_dp
		{
			get { return (double)GetValue(KameraZoom_dpProperty); }
			set { SetValue(KameraZoom_dpProperty, value); }
		}
		/// <summary>
		/// Vektor pozicije kamere na sferi polumjera 1 nakon rotacije po Z i X osi. Duljina vektora je uvijek = 1. Kamera LookDirection je suprotnog smjera.
		/// </summary>
		public Vector3D KameraRotacijaVektor
		{
			get
			{
				return KamRotacijaVektor;
			}
		}
		/// <summary>
		/// Vektor apsolutne pozicije kamere (nakon svih transformacija).
		/// </summary>
		public Vector3D KameraPozicijaVektor
		{
			get
			{
				return KameraTranslacija_dp + KamRotacijaVektor * KameraZoom_dp;
			}
		}
		public Vector3D KameraPozicijaUpVektor
		{
			get
			{
				return KamUp;
			}
		}

		public MatricaRotacije RotacijskaMatricaZXY
		{
			get
			{
				return RotMatricaZXY;
			}
		}
		public MatricaRotacije RotacijskaMatricaZXY_i
		{
			get
			{
				return RotMatricaZXY_i;
			}
		}
		/*
		/// <summary>
		/// Matrica transformacije. Kamera se ne moze upravljati preko nje.
		/// </summary>
		public Matrix3D KameraMatrixTransform
		{
			get
			{
				return KamMatrix;
			}
		}*/

		// jednu od ovih kamera staviti u viewport3d

		public OrthographicCamera KameraOrt
		{
			get
			{
				return KamOrt;
			}
		}
		public PerspectiveCamera KameraPer
		{
			get
			{
				return KamPer;
			}
		}

		// ako je vazno da parent objekt zna da su se parametri kamere promijenili
		/// <summary>
		/// Prilikom promjene nekog parametra kamere dize se ovaj event.
		/// </summary>
		public event EventHandler EVENT_PromjenaParametaraKamere;

		// private properties
		private OrthographicCamera KamOrt = new OrthographicCamera();
		private PerspectiveCamera KamPer = new PerspectiveCamera();
		private static Point3D KamDefPoz = new Point3D(0, -1, 0);
		private static Point3D KamUpDefPoz = new Point3D(0, 0, 1);
		private Vector3D KamRotacijaVektor = (Vector3D)KamDefPoz; // def: (0, -1, 0)
		private Vector3D KamUp = (Vector3D)KamUpDefPoz; // def: (0, 0, 0.5)

		private MatricaRotacije RotMatricaZXY = new MatricaRotacije();
		private MatricaRotacije RotMatricaZXY_i = new MatricaRotacije();

		//private Matrix3D KamMatrix = Matrix3D.Identity;
		
		public RotaKamera()
		{
			// neke osnovne vrijednosti kamera

			KamOrt.Position = KamDefPoz;
			KamOrt.UpDirection = KamUp;
			KamOrt.LookDirection = -KamRotacijaVektor;
			KamOrt.FarPlaneDistance = 50;
			KamOrt.NearPlaneDistance = 0.001;
			KamOrt.Width = 1;

			KamPer.Position = KamDefPoz;
			KamPer.UpDirection = KamUp;
			KamPer.LookDirection = -KamRotacijaVektor;
			KamPer.FarPlaneDistance = 50;
			KamPer.NearPlaneDistance = 0.1;
			KamPer.FieldOfView = 60;
		}

		// za apsolutnu promjenu neke vrijednosti direktno unijeti u odgovarajuci DependencyProperty
		/// <summary>
		/// Relativna rotacija kamere za neki kut. Rotacija se vrsi po z, x i y osima.
		/// </summary>
		/// <param name="xyzStupnjevi">Vrijednosti kuteva rotacija oko x, oko y i oko z sacuvani u, redom, Point3D.X,Point3D.Y,Point3D.Z </param>
		public void KamRotateRelativ(Point3D xyzStupnjevi)
		{
			KameraRotacijaKuteviXYZ_dp = new Point3D(
				KameraRotacijaKuteviXYZ_dp.X + xyzStupnjevi.X,
				KameraRotacijaKuteviXYZ_dp.Y + xyzStupnjevi.Y,
				KameraRotacijaKuteviXYZ_dp.Z + xyzStupnjevi.Z);
		}
		/// <summary>
		/// Relativna translacija kamere za neki vektor.
		/// </summary>
		/// <param name="xyzPomak">Vektor translacije</param>
		public void KamTranslateRelativ(Vector3D xyzPomak)
		{
			// relativna translacija
			KameraTranslacija_dp = new Vector3D(
				KameraTranslacija_dp.X + xyzPomak.X,
				KameraTranslacija_dp.Y + xyzPomak.Y,
				KameraTranslacija_dp.Z + xyzPomak.Z);

		}
		/// <summary>
		/// Relativna promjena zooma kamere.
		/// </summary>
		/// <param name="zoomKoef">Vrijednost promjene zooma kamere.</param>
		public void KamZoomRelativ(double zoom)
		{
			KameraZoom_dp += zoom;
		}

		// kanvas -> kanvas2D
		/// <summary>
		/// Vraca 2D koordinatu kursora u kanvasu ovisno o velicini kanvasa i parametrima kamere. Ishodiste xy sustava je srediste kanvasa.
		/// </summary>
		/// <param name="_kanvasActualWidth">Stvarna sirina kanvasa.</param>
		/// <param name="_kanvasActualHeight">Stvarna visina kanvasa.</param>
		/// <param name="_kanvasPozicijaKursora">Pozicija kursora u kanvasu: System.Windows.Input.Mouse.GetPosition()</param>
		/// <param name="_aktivnaPerspektiva">Staviti "True" ako je perspektivna kamera aktivna (uracunava i njezinu deformaciju). Vazno kod stavljanja novih objekata na scenu preko kanvasa.</param>
		/// <returns></returns>
		public Point Projektor_2DKanvasViaKanvasKursorPoz(double _kanvasActualWidth, double _kanvasActualHeight, Point _kanvasPozicijaKursora, bool _aktivnaPerspektiva)
		{
			// 1. tocku kanvasa stavlja u interval od 0 do 1 (dijeli sa sirinom)
			// 2. stavi ishodiste (0,0) u centar kanvasa (pomakne za 0.5)
			// 3. ako kanvas nije kvadrat, prilagodi interval za y os
			// 4. prosiri interval na zoom koeficijent

			Point p2d = _kanvasPozicijaKursora;

			//koordinate su povecane za zoom faktor
			p2d.X = ((p2d.X / _kanvasActualWidth) - 0.5) * KameraZoom_dp;
			p2d.Y = ((-(p2d.Y / _kanvasActualHeight) + 0.5) * (_kanvasActualHeight / _kanvasActualWidth)) * KameraZoom_dp;


			// perspektivna kamera:
			if (_aktivnaPerspektiva)
			{
				Vector3D vektorPozicijeKam = KameraTranslacija_dp + KamRotacijaVektor * KameraZoom_dp;

				double udaljenostCentralnePloheOdKamere = Vector3D.DotProduct(vektorPozicijeKam, KamRotacijaVektor);

				p2d.X = p2d.X * udaljenostCentralnePloheOdKamere * 2 * prostorneRotacije.tngStu(KamPer.FieldOfView / 2) / KameraZoom_dp;
				p2d.Y = p2d.Y * udaljenostCentralnePloheOdKamere * 2 * prostorneRotacije.tngStu(KamPer.FieldOfView / 2) / KameraZoom_dp;
			}

			return p2d;
		}

		// kanvas2D -> viewport3D
		/// <summary>
		/// Vraca 3D koordinatu za viewport preko 2D koordinate kanvasa.
		/// 3D tocka se nalazi na plohi koja prolazi kroz ishodiste sustava i zarotirana je prema kameri.
		/// </summary>
		/// <param name="_kanvas2DKoordinata">2D koordinata neke tocke kanvasa (ne proslijediti Mouse.GetPosition()).</param>
		/// <returns></returns>
		public Point3D Projektor_3DViewportVia2DKanvas(Point _kanvas2DKoordinata)
		{
			// tocka na plohi u koju gleda kamera, ploha prolazi kroz centar sustava

			Point3D p3d = (Point3D)
				(RotMatricaZXY *  new Vector3D(_kanvas2DKoordinata.X, 0, _kanvas2DKoordinata.Y));
			/*
			Point3D p3d = prostorneRotacije.ZXY(
				new Point3D(_kanvas2DKoordinata.X, 0, _kanvas2DKoordinata.Y),
				KameraRotacijaKuteviXYZ_dp.Z,
				KameraRotacijaKuteviXYZ_dp.X,
				KameraRotacijaKuteviXYZ_dp.Y);
			*/
			Vector3D vektorPozicijeKam = KameraTranslacija_dp + KamRotacijaVektor * KameraZoom_dp;
			// udaljenost centralne plohe od kamere (ploha u koju kamera gleda a prolazi kroz (0,0,0)
			double udaljenostOdKamere = Vector3D.DotProduct(vektorPozicijeKam, KamRotacijaVektor);

			Vector3D vektorTranslacije = KameraTranslacija_dp + KamRotacijaVektor * (KameraZoom_dp - udaljenostOdKamere);

			p3d = Point3D.Add(p3d, vektorTranslacije);

			return p3d;
		}
		/// <summary>
		/// Vraca 3D koordinatu za viewport preko 2D koordinate kanvasa.
		/// 3D tocka je ortogonalno udaljena od kamere za neku vrijednost
		/// </summary>
		/// <param name="_kanvas2DKoordinata">2D koordinata neke tocke kanvasa (ne proslijediti Mouse.GetPosition()).</param>
		/// <param name="_udaljenostOdKamere">Fiksna ortogonalna udaljenost od kamere na kojoj se nalazi 3D tocka.</param>
		/// <returns></returns>
		public Point3D Projektor_3DViewportVia2DKanvas(Point _kanvas2DKoordinata, double _udaljenostOdKamere)
		{
			// tocka na plohi u koju gleda kamera, ploha prolazi kroz centar sustava

			Point3D p3d = (Point3D)
				(RotMatricaZXY * new Vector3D(_kanvas2DKoordinata.X, 0, _kanvas2DKoordinata.Y));
			/*
			Point3D p3d = prostorneRotacije.ZXY(
				new Point3D(_kanvas2DKoordinata.X, 0, _kanvas2DKoordinata.Y),
				KameraRotacijaKuteviXYZ_dp.Z,
				KameraRotacijaKuteviXYZ_dp.X,
				KameraRotacijaKuteviXYZ_dp.Y);
			*/
			// tocka se translatira prema kameri na odredjenu udaljenost od nje
			Vector3D vektorTranslacije = KameraTranslacija_dp + KamRotacijaVektor * (KameraZoom_dp - _udaljenostOdKamere);

			p3d = Point3D.Add(p3d, vektorTranslacije);

			return p3d;
		}

		/// <summary>
		/// Vraca Vektor3D koji je Vektor2D (napravljen na kanvasu) ocitan u koordinatama glavnog sustava.
		/// Korisno za 3D translacije objekta preko kanvasa.
		/// </summary>
		/// <param name="_v2dKanvasXY">Vektor2D u sustavu kamere kojeg treba ocitati u glavnom sustavu. X je desno, Y je gore</param>
		/// <returns></returns>
		public Vector3D Projektor_v3dTranslateViaKanvasKoord(Vector _v2dKanvasXY)
		{
			// malo jednostavnije od Projektor_3DViewportVia2DKanvas
			// ovisno o rotaciji kamere translatiraj po potrebnim osima

			Point3D xyz_offset = (Point3D)
				(RotMatricaZXY * new Vector3D(_v2dKanvasXY.X, 0, _v2dKanvasXY.Y));
			/*
			Point3D xyz_offset = prostorneRotacije.ZXY(
				new Point3D(_v2dKanvasXY.X, 0, _v2dKanvasXY.Y),
				KameraRotacijaKuteviXYZ_dp.Z,
				KameraRotacijaKuteviXYZ_dp.X,
				KameraRotacijaKuteviXYZ_dp.Y);
			*/
			return new Vector3D(xyz_offset.X, xyz_offset.Y, xyz_offset.Z);
		}
		/// <summary>
		/// Vraca Vektor3D koji je Vektor2D (napravljen na kanvasu) ocitan u koordinatama glavnog sustava.
		/// </summary>
		/// <param name="_v3dKanvasXYZ">Vektor3D u sustavu kamere kojeg treba ocitati u glavnom sustavu. X je desno, Y je gore, Z komponenta izlazi iz kanvasa.</param>
		/// <returns></returns>
		public Vector3D Projektor_v3dTranslateViaKanvasKoord(Vector3D _v3dKanvasXYZ)
		{
			// malo jednostavnije od Projektor_3DViewportVia2DKanvas

			// _v3dKanvasXYZ.Z je os koja ulazi u kanvas, sluzi za udaljavanje kamere od centra

			// ovisno o rotaciji kamere translatiraj po potrebnim osima

			Point3D xyz_offset = (Point3D)
				(RotMatricaZXY * new Vector3D(_v3dKanvasXYZ.X, _v3dKanvasXYZ.Z, _v3dKanvasXYZ.Y));
			/*
			Point3D xyz_offset = prostorneRotacije.ZXY(
				new Point3D(_v3dKanvasXYZ.X, _v3dKanvasXYZ.Z, _v3dKanvasXYZ.Y),
				KameraRotacijaKuteviXYZ_dp.Z,
				KameraRotacijaKuteviXYZ_dp.X,
				KameraRotacijaKuteviXYZ_dp.Y);
			*/
			return new Vector3D(xyz_offset.X, xyz_offset.Y, xyz_offset.Z);
		}

		// viewport -> kanvas (kruzic selekcije)
		/// <summary>
		/// Vraca projekciju 3D tocke viewporta na kanvas.
		/// Inverzna funkcija od Koord3dViewportViaKanvas2dKoord.
		/// </summary>
		/// <param name="_viewport3DKoord">Koordinata 3D tocke koju se zeli projicirati na kanvas.</param>
		/// <param name="_aktivnaPerspektiva">Staviti "True" ako je perspektivna kamera aktivna (uracunava i njezinu deformaciju).</param>
		/// <returns></returns>
		public Point Projektor_2DKanvasVia3DViewport(Point3D _viewport3DKoord, bool _aktivnaPerspektiva)
		{
			Point3D p3d = Point3D.Add(_viewport3DKoord, -KameraPozicijaVektor);
			//Point3D p3d = _viewport3DKoord;

			p3d = (Point3D)(RotMatricaZXY_i * (Vector3D)p3d);

			/*
			p3d = prostorneRotacije.ZXY_inverz(
				new Point3D(p3d.X, p3d.Y, p3d.Z),
				KameraRotacijaKuteviXYZ_dp.Z,
				KameraRotacijaKuteviXYZ_dp.X,
				KameraRotacijaKuteviXYZ_dp.Y);
			*/
			Point p2d = new Point(p3d.X, p3d.Z);

			if (_aktivnaPerspektiva)
			{
				double udaljenostSelObjodKamere = Projektor_OrtoUdaljenostTockeOdKamere(_viewport3DKoord);

				if (udaljenostSelObjodKamere != 0)
				{
					p2d.X = p2d.X * KameraZoom_dp / (udaljenostSelObjodKamere * 2 * prostorneRotacije.tngStu(KamPer.FieldOfView / 2));
					p2d.Y = p2d.Y * KameraZoom_dp / (udaljenostSelObjodKamere * 2 * prostorneRotacije.tngStu(KamPer.FieldOfView / 2));
				}
			}

			return p2d;
		}
		/// <summary>
		/// Vraca 2D tocku (u pixelima) koju framework kontrole mogu koristiti (gornji-lijevi kut ima koord(0,0) i raste prema dolje-desno).
		/// </summary>
		/// <param name="_kanvasActualWidth">Stvarna sirina kanvasa.</param>
		/// <param name="_kanvasActualHeight">Stvarna visina kanvasa.</param>
		/// <param name="_kanvas2DKoord">2D koordinata kanvasa koju treba transformirati.</param>
		/// <returns></returns>
		public Point Projektor_KursorPozVia2DKanvas(double _kanvasActualWidth, double _kanvasActualHeight, Point _kanvas2DKoord)
		{
			Point p2d = _kanvas2DKoord;

			p2d.X = (p2d.X / KameraZoom_dp + 0.5)*_kanvasActualWidth;
			p2d.Y = -((p2d.Y * _kanvasActualWidth) / (KameraZoom_dp * _kanvasActualHeight) - 0.5) * _kanvasActualHeight;

			return p2d;
		}
		/// <summary>
		/// Vraca 2D tocku (u pixelima) koju framework kontrole mogu koristiti (gornji-lijevi kut ima koord(0,0) i raste prema dolje-desno).
		/// </summary>
		/// <param name="_viewport3DKoord">Koordinata 3D tocke koju se zeli projicirati na kanvas.</param>
		/// <param name="_aktivnaPerspektiva">Staviti "True" ako je perspektivna kamera aktivna (uracunava i njezinu deformaciju).</param>
		/// <param name="_kanvasActualWidth">Stvarna sirina kanvasa.</param>
		/// <param name="_kanvasActualHeight">Stvarna visina kanvasa.</param>
		/// <returns></returns>
		public Point Projektor_KursorPozVia3DViewport(Point3D _viewport3DKoord, bool _aktivnaPerspektiva, double _kanvasActualWidth, double _kanvasActualHeight)
		{
			return Projektor_KursorPozVia2DKanvas(_kanvasActualWidth, _kanvasActualHeight,
				Projektor_2DKanvasVia3DViewport(_viewport3DKoord, _aktivnaPerspektiva));
		}

		// viewport
		/// <summary>
		/// Racuna ortogonalnu udaljenost kamere i plohe na kojoj je zadana 3dtocka.
		/// </summary>
		/// <param name="_p3d">3d tocka ciju ortogonalnu udaljenost od kamere treba izracunati.</param>
		/// <returns></returns>
		public double Projektor_OrtoUdaljenostTockeOdKamere(Point3D _p3d)
		{
			// ako je udaljenost negativna znaci da kamera ne vidi centar objekta

			double udaljenostObjodKamere = Vector3D.DotProduct(KameraPozicijaVektor, KamRotacijaVektor)
				- Vector3D.DotProduct((Vector3D)_p3d, KamRotacijaVektor);

			return udaljenostObjodKamere;
		}

		// ove funkcije klasa koristi interno
		// pozivaju se automatski kad se promijeni neki public property
		private void KamRotateApsolut(double stupnjeviOkoZ, double stupnjeviOkoX, double stupnjeviOkoY)
		{
			// tocka na sferi polumjera 1 gdje je kamera

			RotMatricaZXY = prostorneRotacije.matrica_ZXY(stupnjeviOkoZ, stupnjeviOkoX, stupnjeviOkoY);
			RotMatricaZXY_i = prostorneRotacije.matrica_ZXY_i(stupnjeviOkoZ, stupnjeviOkoX, stupnjeviOkoY);

			KamRotacijaVektor = (Vector3D)(RotMatricaZXY * (Vector3D)KamDefPoz);
			
			// vektor 'kamera gore' se takodjer mora rotirati s kamerom, time se kamera moze "kotrljati"
			KamUp = (Vector3D)(RotMatricaZXY * (Vector3D)KamUpDefPoz);

		}
		private void KamNamjestiIzGlobalnihParametara()
		{
			// mijenja properties od perspektivne i orto kamere na temelju globalnih parametara ove klase
			/*
			KamMatrix = Matrix3D.Identity;

			Quaternion qz = new Quaternion(new Vector3D(0, 0, 1), KameraRotacijaKuteviXYZ_dp.Z);
			Quaternion qx = new Quaternion(new Vector3D(1, 0, 0), KameraRotacijaKuteviXYZ_dp.X);
			Quaternion qy = new Quaternion(new Vector3D(0, 1, 0), KameraRotacijaKuteviXYZ_dp.Y);
			Quaternion qzxy = qz * qx * qy;

			KamMatrix.Translate(KameraPozicijaVektor);
			KamMatrix.RotateAt(qzxy, (Point3D)KameraPozicijaVektor);


			// preko transform propertia
			MatrixTransform3D mt3 = new MatrixTransform3D(KamMatrix);
			//KameraPozicijaVektor;
			KamOrt.Transform = mt3;
			KamOrt.Width = KameraZoom_dp;
			KamPer.Transform = mt3;
			*/

			// bez matrica
			
			KamOrt.Position = (Point3D)KameraPozicijaVektor; 
			KamOrt.LookDirection = -KamRotacijaVektor;
			KamOrt.UpDirection = KamUp;
			KamOrt.Width = KameraZoom_dp;

			KamPer.Position = (Point3D)KameraPozicijaVektor;
			KamPer.LookDirection = -KamRotacijaVektor;
			KamPer.UpDirection = KamUp;
			

			EVENT_PromjenaParametaraKamere(this, EventArgs.Empty);
		}

		// standardni 2d pogledi
		public void KamPredefiniraneRotacije_Nacrt()
		{
			KameraTranslacija_dp = new Vector3D(0, 0, 0);
			KameraRotacijaKuteviXYZ_dp = new Point3D(0, 0, 0);
		}
		public void KamPredefiniraneRotacije_Straga()
		{
			KameraTranslacija_dp = new Vector3D(0, 0, 0);
			KameraRotacijaKuteviXYZ_dp = new Point3D(0, 0, 180);
		}
		public void KamPredefiniraneRotacije_Tlocrt()
		{
			KameraTranslacija_dp = new Vector3D(0, 0, 0);
			KameraRotacijaKuteviXYZ_dp = new Point3D(-90, 0, 0);
		}
		public void KamPredefiniraneRotacije_Odozdo()
		{
			KameraTranslacija_dp = new Vector3D(0, 0, 0);
			KameraRotacijaKuteviXYZ_dp = new Point3D(90, 0, 0);
		}
		public void KamPredefiniraneRotacije_Bokocrt()
		{
			KameraTranslacija_dp = new Vector3D(0, 0, 0);
			KameraRotacijaKuteviXYZ_dp = new Point3D(0, 0, 90);
		}
		public void KamPredefiniraneRotacije_Slijeva()
		{
			KameraTranslacija_dp = new Vector3D(0, 0, 0);
			KameraRotacijaKuteviXYZ_dp = new Point3D(0, 0, -90);
		}
		public void KamPredefiniraneRotacije_Perspektiva()
		{
			KameraTranslacija_dp = new Vector3D(0, 0, 0);
			KameraRotacijaKuteviXYZ_dp = new Point3D(-45, 0, 45);
		}

		// animacija preko frameova
		public void KamAnimiraj(KamFrame f, double trajanje)
		{
			if (f != null)
			{
				Duration dur = new Duration(TimeSpan.FromMilliseconds(trajanje));

				Point3D kamRotKut_trenutno = KameraRotacijaKuteviXYZ_dp;
				Point3D kamRotKut_frame = f.KameraRotacijaKuteviXYZ;
				Point3D kamRotKut_najkraci = f.KameraRotacijaKuteviXYZ;

				// ako treba napraviti rotaciju za vise od pola kruga vrti u suprotnom smjeru
				double z = kamRotKut_frame.Z - kamRotKut_trenutno.Z;
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

				//FLAG_bKameraAnimating = true;

				this.BeginAnimation(KameraTranslacijaProperty, anim_trans);
				this.BeginAnimation(KameraRotacijaKuteviXYZProperty, anim_rot);
				this.BeginAnimation(KameraZoom_dpProperty, anim_zoom);
			}
		}
		private void anim_Completed(object sender, EventArgs e)
		{
			// sacuvaj transformaciju animacije kao lokalnu vrijednost
			this.KameraTranslacija_dp = this.KameraTranslacija_dp;
			this.KameraRotacijaKuteviXYZ_dp = this.KameraRotacijaKuteviXYZ_dp;
			this.KameraZoom_dp = this.KameraZoom_dp;

			// makni animaciju da se opet mogu mijenjati lokalne vrijednosti
			this.BeginAnimation(KameraTranslacijaProperty, null);
			this.BeginAnimation(KameraRotacijaKuteviXYZProperty, null);
			this.BeginAnimation(KameraZoom_dpProperty, null);

			//FLAG_bKameraAnimating = false;
		}
	}

	// ovdje se spremaju potrebne transformacije kamere potrebne za fiksne tocke animacije
	public class KamFrame
	{
		public string ime = "KamFrame";

		public Vector3D KameraTranslacija = new Vector3D(0, 0, 0);
		public Point3D KameraRotacijaKuteviXYZ = new Point3D(0, 0, 0);
		public double KameraZoom = 1;

		public KamFrame(Vector3D _KameraTranslacija, Point3D _KameraRotacijaKuteviZXY, double _KameraZoom)
		{
			KameraTranslacija = _KameraTranslacija;
			KameraRotacijaKuteviXYZ = _KameraRotacijaKuteviZXY;
			KameraZoom = _KameraZoom;
		}
		public KamFrame(Vector3D _KameraTranslacija, Point3D _KameraRotacijaKuteviZXY, double _KameraZoom, string _ime)
		{
			ime = _ime;
			KameraTranslacija = _KameraTranslacija;
			KameraRotacijaKuteviXYZ = _KameraRotacijaKuteviZXY;
			KameraZoom = _KameraZoom;
		}
		public override string ToString()
		{
			return ime;
		}
	}


}
public class MatricaRotacije
{
	public double[,] matrica;

	public MatricaRotacije()
	{
		matrica = new double[3, 3]{
							{1, 0, 0},
							{0, 1, 0},
							{0, 0, 1}};
	}

	public MatricaRotacije(double kutStupnjevi, char os)
	{
		double kutRad = (kutStupnjevi / 180) * Math.PI;
		double sin = Math.Sin(kutRad);
		double cos = Math.Cos(kutRad);

		switch (os)
		{
			case 'x':
			case 'X':
				matrica = new double[3, 3]{
							{ 1,  0 ,   0  },
							{ 0, cos, -sin },
							{ 0, sin,  cos }};

				break;
			case 'y':
			case 'Y':
				matrica = new double[3, 3]{
							{  cos, 0, sin },
							{   0,  1,  0  },
							{ -sin, 0, cos }};
				break;
			case 'z':
			case 'Z':
				matrica = new double[3, 3]{
							{ cos, -sin, 0 },
							{ sin,  cos, 0 },
							{  0,    0,  1 }};
				break;

			default:
				matrica = new double[3, 3]{
							{1, 0, 0},
							{0, 1, 0},
							{0, 0, 1}};

				break;
		}
	}

	public MatricaRotacije(Matrix3D mx3)
	{
		// iz bog zna kojeg razloga trebam transponirati Matrix3D
		// cini se da wpf ima matricu prilagodjenu za mnozenje vektora zdesna
		matrica = new double[3, 3]{
							{mx3.M11, mx3.M21, mx3.M31},
							{mx3.M12, mx3.M22, mx3.M32},
							{mx3.M13, mx3.M23, mx3.M33}};
	}

	public Matrix3D VratiMatrix3D()
	{
		return new Matrix3D(
			matrica[0, 0], matrica[0, 1], matrica[0, 2], 0,
			matrica[1, 0], matrica[1, 1], matrica[1, 2], 0,
			matrica[2, 0], matrica[2, 1], matrica[2, 2], 0,
			0, 0, 0, 1);
	}

	public static MatricaRotacije PomnoziMatrice(MatricaRotacije m_lijeva, MatricaRotacije m_desna)
	{
		MatricaRotacije m_tmp = new MatricaRotacije();

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				m_tmp.matrica[i, j] = 0;
				for (int k = 0; k < 3; k++)
				{
					m_tmp.matrica[i, j] += m_lijeva.matrica[i, k] * m_desna.matrica[k, j];
				}
			}
		}
		return m_tmp;
	}
	public static Vector3D PomnoziVektorMatricom(MatricaRotacije m, Vector3D v)
	{
		double[] v_tmp = new double[3] { 0, 0, 0 };
		double[] v_orig = new double[3] { v.X, v.Y, v.Z };

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				v_tmp[i] += m.matrica[i, j] * v_orig[j];
			}
		}

		return new Vector3D(v_tmp[0], v_tmp[1], v_tmp[2]);

	}
	public static MatricaRotacije operator *(MatricaRotacije m1, MatricaRotacije m2)
	{
		return PomnoziMatrice(m1, m2);
	}
	public static Vector3D operator *(MatricaRotacije m, Vector3D v)
	{
		return PomnoziVektorMatricom(m, v);
	}
	public override string ToString()
	{
		string txt = "";

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				txt += matrica[i, j].ToString() + "; ";
			}
			txt += "\n";
		}

		return txt;
		/*
		return matrica[0, 0].ToString() + "; " + matrica[0, 1].ToString() + "; " + matrica[0, 2].ToString() + "; " + matrica[0, 3].ToString() + "\n"
			 + matrica[1, 0].ToString() + "; " + matrica[1, 1].ToString() + "; " + matrica[1, 2].ToString() + "; " + matrica[1, 3].ToString() + "\n"
			 + matrica[2, 0].ToString() + "; " + matrica[2, 1].ToString() + "; " + matrica[2, 2].ToString() + "; " + matrica[2, 3].ToString() + "\n"
			 + matrica[3, 0].ToString() + "; " + matrica[3, 1].ToString() + "; " + matrica[3, 2].ToString() + "; " + matrica[3, 3].ToString() + "\n";
		*/
	}
}

public class Matrica4x4
{
	// ne sluzi trenutno nicemu, neka stoji

	public double[,] matrica;

	public Matrica4x4()
	{
		matrica = new double[4, 4]{
							{1, 0, 0, 0},
							{0, 1, 0, 0},
							{0, 0, 1, 0},
							{0, 0, 0, 1}};
	}

	public Matrica4x4(double kutStupnjevi, char os)
	{
		double kutRad = (kutStupnjevi / 180) * Math.PI;
		double sin = Math.Sin(kutRad);
		double cos = Math.Cos(kutRad);

		switch (os)
		{
			case 'x':
			case 'X':
				matrica = new double[4, 4]{
							{ 1,  0 ,   0 , 0 },
							{ 0, cos, -sin, 0 },
							{ 0, sin,  cos, 0 },
							{ 0,  0 ,   0 , 1 }};

				break;
			case 'y':
			case 'Y':
				matrica = new double[4, 4]{
							{  cos, 0, sin, 0 },
							{   0,  1,  0 , 0 },
							{ -sin, 0, cos, 0 },
							{   0,  0,  0 , 1 }};
				break;
			case 'z':
			case 'Z':
				matrica = new double[4, 4]{
							{ cos, -sin, 0, 0 },
							{ sin,  cos, 0, 0 },
							{  0,    0 , 1, 0 },
							{  0,    0 , 0, 1 }};
				break;

			default:
				matrica = new double[4, 4]{
							{1, 0, 0, 0},
							{0, 1, 0, 0},
							{0, 0, 1, 0},
							{0, 0, 0, 1}};

				break;
		}
	}
	/*
	public Matrix3D VratiMatrix3D()
	{
		return new Matrix3D(
			matrica[0, 0], matrica[0, 1], matrica[0, 2], matrica[0, 3],
			matrica[1, 0], matrica[1, 1], matrica[1, 2], matrica[1, 3],
			matrica[2, 0], matrica[2, 1], matrica[2, 2], matrica[2, 3],
			matrica[3, 0], matrica[3, 1], matrica[3, 2], matrica[3, 3]);
	}
	*/
	public static Matrica4x4 PomnoziMatrice(Matrica4x4 m_lijeva, Matrica4x4 m_desna)
	{
		Matrica4x4 m_tmp = new Matrica4x4();

		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				m_tmp.matrica[i, j] = 0;
				for (int k = 0; k < 4; k++)
				{
					m_tmp.matrica[i, j] += m_lijeva.matrica[i, k] * m_desna.matrica[k, j];
				}
			}
		}
		return m_tmp;
	}
	public static Vector3D PomnoziVektorMatricom(Matrica4x4 m, Vector3D v)
	{
		double[] v_tmp = new double[4] { 0, 0, 0, 0 };
		double[] v_orig = new double[4] { v.X, v.Y, v.Z, 1 };

		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				v_tmp[i] += m.matrica[i, j] * v_orig[j];
			}
		}

		return new Vector3D(v_tmp[0], v_tmp[1], v_tmp[2]);

	}
	public static Matrica4x4 operator *(Matrica4x4 m1, Matrica4x4 m2)
	{
		return PomnoziMatrice(m1, m2);
	}
	public static Vector3D operator *(Matrica4x4 m, Vector3D v)
	{
		return PomnoziVektorMatricom(m, v);
	}
	public override string ToString()
	{
		string txt = "";

		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				txt += matrica[i, j].ToString() + "; ";
			}
			txt += "\n";
		}

		return txt;
		/*
		return matrica[0, 0].ToString() + "; " + matrica[0, 1].ToString() + "; " + matrica[0, 2].ToString() + "; " + matrica[0, 3].ToString() + "\n"
			 + matrica[1, 0].ToString() + "; " + matrica[1, 1].ToString() + "; " + matrica[1, 2].ToString() + "; " + matrica[1, 3].ToString() + "\n"
			 + matrica[2, 0].ToString() + "; " + matrica[2, 1].ToString() + "; " + matrica[2, 2].ToString() + "; " + matrica[2, 3].ToString() + "\n"
			 + matrica[3, 0].ToString() + "; " + matrica[3, 1].ToString() + "; " + matrica[3, 2].ToString() + "; " + matrica[3, 3].ToString() + "\n";
		*/
	}
}

// matematicka klasa cije funkc vracaju poziciju nove tocke ovisno o kutevima rotacija
public static class prostorneRotacije
{
	// ovo sluzi samo radi lakseg citanja formula
	private static double sin(double x_radijana)
	{
		return Math.Sin(x_radijana);
	}
	private static double cos(double x_radijana)
	{
		return Math.Cos(x_radijana);
	}

	public static double sinStu(double x_stupnjeva)
	{
		return Math.Sin(stupnjevi_u_radijane(x_stupnjeva));
	}
	public static double cosStu(double x_stupnjeva)
	{
		return Math.Cos(stupnjevi_u_radijane(x_stupnjeva));
	}
	public static double tngStu(double x_stupnjeva)
	{
		return Math.Tan(stupnjevi_u_radijane(x_stupnjeva));
	}


	public static double stupnjevi_u_radijane(double x_stupnjeva)
	{
		return (x_stupnjeva / 180) * Math.PI;
	}
	public static double radijani_u_stupnjeve(double x_radijana)
	{
		return (x_radijana / Math.PI) * 180;
	}

	public static MatricaRotacije matrica_Z(double stupnjeva_okoZ)
	{
		return new MatricaRotacije(stupnjeva_okoZ, 'z');
	}
	public static MatricaRotacije matrica_ZX(double stupnjeva_okoZ, double stupnjeva_okoX)
	{
		MatricaRotacije mx = new MatricaRotacije(stupnjeva_okoX, 'x');
		MatricaRotacije mz = new MatricaRotacije(stupnjeva_okoZ, 'z');

		return mz * mx;
	}
	public static MatricaRotacije matrica_ZXY(double stupnjeva_okoZ, double stupnjeva_okoX, double stupnjeva_okoY)
	{
		MatricaRotacije mx = new MatricaRotacije(stupnjeva_okoX, 'x');
		MatricaRotacije my = new MatricaRotacije(stupnjeva_okoY, 'y');
		MatricaRotacije mz = new MatricaRotacije(stupnjeva_okoZ, 'z');

		return mz * (mx * my);
	}
	public static MatricaRotacije matrica_ZXY_i(double stupnjeva_okoZ, double stupnjeva_okoX, double stupnjeva_okoY)
	{
		MatricaRotacije mx = new MatricaRotacije(-stupnjeva_okoX, 'x');
		MatricaRotacije my = new MatricaRotacije(-stupnjeva_okoY, 'y');
		MatricaRotacije mz = new MatricaRotacije(-stupnjeva_okoZ, 'z');

		return my * (mx * mz);
	}
}