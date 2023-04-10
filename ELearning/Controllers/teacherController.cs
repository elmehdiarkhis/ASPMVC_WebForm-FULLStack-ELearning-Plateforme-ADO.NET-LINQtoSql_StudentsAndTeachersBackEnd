using ELearning.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Net.WebRequestMethods;

namespace ELearning.Controllers
{
    public class teacherController : Controller
    {
        // GET: teacher
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(string username, string pass)
        {
            try
            {
                myDataBaseDataContext myDataBase = new myDataBaseDataContext();

                Teacher resSelectCheck = (from _Teachers in myDataBase.Teachers
                                     where _Teachers.userName == username && _Teachers.pass == pass
                                     select _Teachers).SingleOrDefault<Teacher>();
                if(resSelectCheck == null)
                {
                    ViewBag.h1Error = "incorrect teacher userName or password";
                    return View("login");
                }
                else
                {
                    GlobalVar.teacherId = resSelectCheck.idTeacher;
                    return RedirectToAction("chooseNe");
                }
            }
            catch (Exception ex)
            {

            }

            return View();

        }


        // GET: teacher
        public ActionResult chooseNe()
        {
            //requete de touts les Niveau d'etude

            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            var resNvEtud = from _JoinTeacherNes in myDataBase.JoinTeacherNEs
                            join _niveauEtudeTables in myDataBase.niveauEtudeTables
                            on _JoinTeacherNes.idNe equals _niveauEtudeTables.Id

                            where _JoinTeacherNes.idTeacher == GlobalVar.teacherId
                            select _niveauEtudeTables.niveauEtude;

            List<String> myListNivEtudStr = resNvEtud.ToList<String>();

            ViewBag.myListNivEtudStr = myListNivEtudStr;

            return View();
        }


        [HttpPost]
        public ActionResult chooseNe(string nivEtud)
        {
            //requete course where nivEtud && teacherId

            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            var resultat = from j in myDataBase.JoinTeacherCourses
                           join c in myDataBase.CoursesTables
                           on j.idCourse equals c.Id

                           where j.idTeacher == GlobalVar.teacherId && c.NiveauEtude == nivEtud
                           select c;

            List<CoursesTable> myListOfTeacherCourses = resultat.ToList<CoursesTable>();

            ViewBag.myListOfTeacherCourses = myListOfTeacherCourses;

            return View("courses");
        }



        // GET: 
        public ActionResult courseTeacherDetails(int id)
        {
            GlobalVar.idCourseT = id;

            VideoPdfExo videoPdfExoTables = selectAll(id);


            return View("courseTeacherDetails",videoPdfExoTables);
        }

        public VideoPdfExo selectAll(int id)
        {
            //Recuperer la Liste des Pdf==========================================================
            string r = "";
            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            var resultat = from JoinCoursesPDFs in myDataBase.JoinCoursesPDFs

                           join CoursesTables in myDataBase.CoursesTables
                           on JoinCoursesPDFs.idCourse equals CoursesTables.Id

                           join PDFs in myDataBase.PDFs
                           on JoinCoursesPDFs.idPdf equals PDFs.IdPdf

                           where CoursesTables.Id == id && PDFs.idTeacher == GlobalVar.teacherId
                           select PDFs;

            List<PDF> lstPdfTable = resultat.ToList<PDF>();

            //=====================================================================================




            //Recuperer la Liste des Exercices==========================================================
            var resultat_2 = from JoinCoursesExercices in myDataBase.JoinCoursesExercices

                             join CoursesTables in myDataBase.CoursesTables
                             on JoinCoursesExercices.idCourse equals CoursesTables.Id

                             join Exercices in myDataBase.Exercices
                             on JoinCoursesExercices.idExercice equals Exercices.IdExercice

                             where CoursesTables.Id == id && Exercices.idTeacher == GlobalVar.teacherId
                             select Exercices;

            List<Exercice> lstExoTable = resultat_2.ToList<Exercice>();

            //=====================================================================================


            //Recuperer la Liste des urlVideos==========================================================
            var resultat_3 = from CourseVideoJencts in myDataBase.CourseVideoJencts

                             join CoursesTables in myDataBase.CoursesTables
                             on CourseVideoJencts.IdCourse equals CoursesTables.Id

                             join VideosTables in myDataBase.VideosTables
                             on CourseVideoJencts.IdVideo equals VideosTables.IdVideo

                             where CoursesTables.Id == id && VideosTables.idTeacher == GlobalVar.teacherId && VideosTables.statut == "prive"
                             select VideosTables;

            List<VideosTable> lstVideoTable = resultat_3.ToList<VideosTable>();

            string tr = "";
            //=====================================================================================


            //Mettre les list dans une seul class pour l'envoyer cote client============================
            VideoPdfExo videoPdfExoTables = new VideoPdfExo();
            videoPdfExoTables.lstPdfTable = lstPdfTable;
            videoPdfExoTables.lstExoTable = lstExoTable;
            videoPdfExoTables.lstVideoTable = lstVideoTable;
            //=================================================================================

            return videoPdfExoTables;
        }





        // GET: 
        public ActionResult showVideo(int id)
        {
            string x = "";

            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            VideosTable videoObj = (from VideosTables in myDataBase.VideosTables
                           where VideosTables.IdVideo == id
                           select VideosTables).SingleOrDefault<VideosTable>();

            ViewBag.viedoObj = videoObj;

            return View("showVideo");
        }


        // GET: 
        public ActionResult showPdf(int id)
        {
            string x = "";

            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            PDF pdfObj = (from PDFs in myDataBase.PDFs
                                    where PDFs.IdPdf == id
                                    select PDFs).SingleOrDefault<PDF>();

            ViewBag.pdfObj = pdfObj;

            return View("showPdf");
        }


        // GET: 
        public ActionResult showExo(int id)
        {
            string x = "";

            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            Exercice exerciceObj = (from Exercices in myDataBase.Exercices
                          where Exercices.IdExercice == id
                          select Exercices).SingleOrDefault<Exercice>();

            ViewBag.exerciceObj = exerciceObj;

            return View();
        }


        // GET: 
        public ActionResult add()
        {
            return View();
        }

        // POST
        [HttpPost]
        public ActionResult add(string selectedOption)
        {
            string x = selectedOption;
            string viewName="";

            if (selectedOption == "Video")
            {
                viewName = "addVideo";
            }
            else if(selectedOption == "Exercice")
            {
                viewName = "addExo";
            }
            else if (selectedOption == "Cours")
            {
                viewName = "addCours";
            }



            return View(viewName);
        }

        // GET: 
        public ActionResult addVideo()
        {
            return View();
        }


        // POST:
        [HttpPost]
        public ActionResult addVideo(string x)
        {
            return View();
        }


        // GET: 
        public ActionResult addExo()
        {
            ViewBag.message = "";
            ViewBag.color = "";
            return View();
        }


        // POST:
        [HttpPost]
        public ActionResult addExo(string titre,string descri,string dateRemise,HttpPostedFileBase file)
        {
            string x = "";

            DateTime dateR = Convert.ToDateTime(Convert.ToDateTime(dateRemise).ToShortDateString());
            if (dateR < DateTime.Now)
            {
                ViewBag.dateError = "Veuillez choisir une Date superieur ou egal a Aujourd'hui";
                return View("addExo");
            }


            try
            {
                //File Upload====
                var path = Path.Combine(Server.MapPath("~/Content/exercices"), file.FileName);
                file.SaveAs(path);
                //==============
            }
            catch (Exception ex)
            {
              
            }
            


            try
            {
                //Insert 1 Exercice=================================================
                myDataBaseDataContext myDataBase = new myDataBaseDataContext();

                Exercice newExo = new Exercice
                {
                    urlPdfExo = file.FileName,
                    idTeacher = GlobalVar.teacherId,
                    //datePublicationExo = getDate() Default
                    dateRemiseExo = dateR,
                    titreExo = titre,
                    DescriExo = descri
                };

                myDataBase.Exercices.InsertOnSubmit(newExo);
                myDataBase.SubmitChanges();
                //==================================================================


                //Insert in JencExoCours =================================================

                //select newGenerated Id
                int idExo = (from exercices in myDataBase.Exercices
                             where exercices.urlPdfExo == file.FileName && exercices.titreExo == titre && exercices.DescriExo==descri
                             select exercices.IdExercice).SingleOrDefault<int>();


                JoinCoursesExercice newJoin = new JoinCoursesExercice
                {
                    idCourse = GlobalVar.idCourseT,
                    idExercice = idExo,
                };
                myDataBase.JoinCoursesExercices.InsertOnSubmit(newJoin);
                myDataBase.SubmitChanges();
                //========================================================================

                //ViewBag.message =nb "l'Exercice a ete ajouter avec succes...";
                //ViewBag.color = "Green";


                VideoPdfExo videoPdfExoTables = selectAll(GlobalVar.idCourseT);
                return View("courseTeacherDetails", videoPdfExoTables);

            }
            catch (Exception ex)
            {
                ViewBag.message = "Une Erreur c'est produite : " + ex.Message;
                ViewBag.color = "Red";
            }
            


            string y = "";

            return View("addExo");
        }



        // GET: 
        public ActionResult addCours()
        {
            ViewBag.message = "";
            ViewBag.color = "";
            return View();
        }


        // POST:
        [HttpPost]
        public ActionResult addCours(string titre, string descri, HttpPostedFileBase filecours)
        {

            string x = "";

     
            try
            {
                //File Upload====
                var path = Path.Combine(Server.MapPath("/Content/pdfs"), filecours.FileName);
                filecours.SaveAs(path);
                //==============


                //Insert 1 Exercice=================================================
                myDataBaseDataContext myDataBase = new myDataBaseDataContext();

                PDF newCoursDoc = new PDF
                {
                    urlPdf = filecours.FileName,
                    idTeacher = GlobalVar.teacherId,
                    //datePublication = default,
                    titreCours = titre,
                    DescriCours = descri

                };

                myDataBase.PDFs.InsertOnSubmit(newCoursDoc);
                myDataBase.SubmitChanges();
                //==================================================================


                //Insert in JencCoursPDF =================================================

                //select newGenerated Id
                int idCoursdoc = (from pdfs in myDataBase.PDFs
                                  where pdfs.urlPdf == filecours.FileName && pdfs.titreCours == titre && pdfs.DescriCours == descri
                                  select pdfs.IdPdf).SingleOrDefault<int>();


                JoinCoursesPDF newJo = new JoinCoursesPDF
                {
                    idPdf = idCoursdoc,
                    idCourse = GlobalVar.idCourseT
                };
                myDataBase.JoinCoursesPDFs.InsertOnSubmit(newJo);
                myDataBase.SubmitChanges();
                //========================================================================

                //ViewBag.message = "l'Exercice a ete ajouter avec succes...";
                //ViewBag.color = "Green";


                VideoPdfExo videoPdfExoTables = selectAll(GlobalVar.idCourseT);
                return View("courseTeacherDetails", videoPdfExoTables);

            }
            catch (Exception ex)
            {
                ViewBag.message = "Une Erreur c'est produite : " + ex.Message;
                ViewBag.color = "Red";
            }


            return View();
        }
        
        public ActionResult deletePdf(int id)
        {
            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            //delete from the Table Jointure========================
            JoinCoursesPDF jcd = (from joinCP in myDataBase.JoinCoursesPDFs
                                  where joinCP.idPdf==id
                                  select joinCP).SingleOrDefault<JoinCoursesPDF>();
            
            myDataBase.JoinCoursesPDFs.DeleteOnSubmit(jcd);
            myDataBase.SubmitChanges();
            //==========================================================

            //delete From the Table===================================
            PDF pdf = (from pdfs in myDataBase.PDFs
                      where pdfs.IdPdf == id 
                      select pdfs).SingleOrDefault<PDF>();

            myDataBase.PDFs.DeleteOnSubmit(pdf);
            myDataBase.SubmitChanges();
            //======================================================


            VideoPdfExo videoPdfExoTables = selectAll(GlobalVar.idCourseT);
            return View("courseTeacherDetails", videoPdfExoTables);
        }

        
        public ActionResult deleteExo(int id)
        {
            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            //delete from the Table Jointure========================
            JoinCoursesExercice jce = (from joinCE in myDataBase.JoinCoursesExercices
                                  where joinCE.idExercice == id
                                  select joinCE).SingleOrDefault<JoinCoursesExercice>();

            myDataBase.JoinCoursesExercices.DeleteOnSubmit(jce);
            myDataBase.SubmitChanges();
            //==========================================================

            //delete From the Table===================================
            Exercice exo = (from exos in myDataBase.Exercices
                       where exos.IdExercice == id
                       select exos).SingleOrDefault<Exercice>();

            myDataBase.Exercices.DeleteOnSubmit(exo);
            myDataBase.SubmitChanges();
            //======================================================


            VideoPdfExo videoPdfExoTables = selectAll(GlobalVar.idCourseT);
            return View("courseTeacherDetails", videoPdfExoTables);
        }




    }
}