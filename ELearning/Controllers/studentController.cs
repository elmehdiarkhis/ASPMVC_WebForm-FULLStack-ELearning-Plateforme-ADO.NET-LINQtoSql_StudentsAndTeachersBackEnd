using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


using System.Data.SqlClient;
using System.Text;
using System.Xml;
using System.Web.Helpers;


//email
using System.Net.Mail;
using System.Net;
using System.Web.Services.Description;

//classes
using ELearning.Models;



namespace ELearning.Controllers
{
    public class studentController : Controller
    {

        //varibale globale
        string userName_g;
        string pass_g;


        // GET: Inscription
        public ActionResult index()
        {
            return View("index");
        }



        // GET: Inscription
        public ActionResult Inscription()
        {
            RemplirSelect();
            return View("inscription");
        }



        // GET: POST
        [HttpPost]
        public ActionResult Inscription(inscription newStudent)
        {
            // checker su l'etudiant existe
            string nom = newStudent.Nom;
            string prenom = newStudent.Prenom;
            string email = newStudent.Email;
            string niveauEtude = newStudent.NiveauEtude;

            string viewName = "";

            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\ELearning\ELearning\App_Data\ELearning.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sql = "SELECT id FROM inscriptionTable WHERE Email='"+email+"'";
                SqlCommand myComm = new SqlCommand(sql, myCon);

                
                SqlDataReader myReader = myComm.ExecuteReader();
                if (myReader.Read())
                {
                    RemplirSelect();
                    ViewBag.h1Message = "Cette Email est deja associee a un compte";
                    viewName = "inscription";
                    myReader.Close();
                }
                else
                {
                    myReader.Close();

                    // generer un UserName + Pass unique > les envoyer par email > Redirect to Login(informer le client to check email)
                    string userName = nom+"_"+prenom;

                    //generate Random Password
                    Random rand = new Random();
                    int stringlen = rand.Next(4, 10);
                    int randValue;
                    string pass = "";
                    char letter;
                    for (int i = 0; i < stringlen; i++)
                    {
                        randValue = rand.Next(0, 26);
                        letter = Convert.ToChar(randValue + 65);
                        pass = pass + letter;
                    }

                    ViewBag.userName = userName;
                    ViewBag.pass = pass;

                    userName_g = "aa";
                    pass_g = "aa";



                    // Insert in two Tables ============
                    string sqlInsertInscription = "INSERT INTO inscriptionTable(Nom,Prenom,Email,NiveauEtude) VALUES('"+nom+"','"+prenom+"','"+email+"','"+niveauEtude+"')";
                    SqlCommand myCommInsertInscription = new SqlCommand(sqlInsertInscription, myCon);
                    myCommInsertInscription.ExecuteNonQuery();

                    string sqlInsertStudent = "INSERT INTO studentTable(Nom,Prenom,Email,NiveauEtude,UserName,Pass) VALUES('"+nom+"','"+prenom+"','"+email+"','"+niveauEtude+"','"+userName+"','"+pass+"')";
                    SqlCommand myCommInsertStudent = new SqlCommand(sqlInsertStudent, myCon);
                    myCommInsertStudent.ExecuteNonQuery();
                    //===================================






                    ////send an Email==========================================================
                    //just pour tester
                    viewName = "login";
                    ViewBag.succesEmail = "Un Email a ete envoyer sur votre courriel Contenant votre mots de pass temporaire";
                    ViewBag.colorEmail = "green";
                    //=============
                    //if (sendMailAfterSignUp(userName, pass, newStudent.Email))
                    //{
                    //    ViewBag.succesEmail = "Un Email a ete envoyer sur votre courriel Contenant votre mots de pass temporaire";
                    //    ViewBag.colorEmail = "green";
                    //    viewName = "login";
                    //}
                    //else
                    //{  
                    //    ViewBag.succesEmail = "Un probleme est survenue lors de l'envoie de l'email";
                    //    ViewBag.colorEmail = "red";
                    //    viewName = "login";
                    //}
                    //=======================================================================

                   

                       
                }
            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {  
                myCon.Close();
            }


            //checker le Closing des Reader
            return View(viewName);

            
        }


        //Get Login
        public ActionResult Login()
        {
            //ViewBag.username = "";
            //ViewBag.pass = "";
            //ViewBag.succesEmail = "";

            

            return View("Login");
        }

        //Post Login
        [HttpPost]
        public ActionResult Login(string username,string pass)
        {
            
            string viewName = "";
            //check if existe in student table 
            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\ELearning\ELearning\App_Data\ELearning.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sql = "SELECT statutFirstLogin,id FROM studentTable WHERE UserName='"+username+"' AND Pass='"+pass+"'";
                SqlCommand myComm = new SqlCommand(sql, myCon);


                SqlDataReader myReader = myComm.ExecuteReader();
                if (myReader.Read()==false)
                {
                    ViewBag.h1Error = "incorrect userName or password";
                    viewName = "login";
                    myReader.Close();
                }
                else
                {
                    GlobalVar.userId = Convert.ToInt32(myReader["id"]);
                    ViewBag.h1Error = "";

                    if (Convert.ToInt32(myReader["statutFirstLogin"]) == 0)
                    {
                        ViewBag.userId = myReader["id"].ToString();
                        viewName = "firstLogin";
                        myReader.Close();                                      
                    }
                    else
                    {
                        //viewName = "courses";
                        return RedirectToAction("courses");
                    }
                   
                    myReader.Close();
                }
            }
            catch(Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }

            return View(viewName);
        }




        

        

        //============================================

        //Get
        public ActionResult firstLogin()
        {
            return View("firstLogin");
        }

        //POST 
        [HttpPost]
        public ActionResult firstLogin(string userId,string oldPass_fc, string newPass_fc,string newPassConfirm_fc)
        {
            string viewname = "";
            int id = Convert.ToInt32(userId);

            

            //check if where id , pass = pass entre
            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\ELearning\ELearning\App_Data\ELearning.mdf;Integrated Security=True");
            try
            {
                myCon.Open();
                string sqlCheck = "SELECT id,UserName FROM studentTable WHERE id="+id+" AND Pass='"+oldPass_fc+"'";
                SqlCommand myCommCheck = new SqlCommand(sqlCheck, myCon);
                SqlDataReader myReaderCheck = myCommCheck.ExecuteReader();

                if (myReaderCheck.Read()==false)
                {
                    viewname = "firstLogin";
                    ViewBag.msgErrorFL = "Ancien mots de pass incorrect, un email a ete envoyer lors du signUp contenant le userName et l'ancien mots de pass";
                    ViewBag.userId = userId;
                }
                else
                {
                    if(newPass_fc == newPassConfirm_fc)
                    {
                        viewname = "login";
                        ViewBag.userName = myReaderCheck["UserName"].ToString();
                        ViewBag.pass = newPass_fc;

                        myReaderCheck.Close();

                        string sqlUpdate = "UPDATE studentTable SET Pass='"+newPass_fc+"',statutFirstLogin=1 WHERE id="+id+";";
                        SqlCommand myCommUpdate = new SqlCommand(sqlUpdate, myCon);
                        myCommUpdate.ExecuteNonQuery();



                        ////send an Email==========================================================
                        //just pour tester
                        ViewBag.succesEmail = "Un Email a ete envoyer sur votre courriel Contenant votre nouveau mots de pass";
                        ViewBag.colorEmail = "green";
                        //=============
                        //if (sendMailAfterSignUp(userName, pass, newStudent.Email))
                        //{
                        //@ViewBag.succesEmail = "Un Email a ete envoyer sur votre courriel Contenant votre nouveau mots de pass";
                        //    ViewBag.colorEmail = "green";
                        //    viewName = "login";
                        //}
                        //else
                        //{  
                        //    ViewBag.succesEmail = "Un probleme est survenue lors de l'envoie de l'email";
                        //    ViewBag.colorEmail = "red";
                        //    viewName = "login";
                        //}
                        //=======================================================================

                    }
                    else
                    {
                        viewname = "firstLogin";
                        ViewBag.msgErrorFL = "la confirmation du mots de pass est incorrect";
                        ViewBag.userId = userId;
                    }
                   
                }
            }catch(Exception ex)
            {

            }
            finally
            {
                myCon.Close();
            }

            return View(viewname);

        }

        //============================================

        // GET: courses
        public ActionResult courses()
        {
            //List
            List<Courses> myCoursesList = new List<Courses>();

            string niveauEtude = "";
            Int32 userId = GlobalVar.userId;

            //requete SELECT niveauEduc From Etudiant Where id=id;

            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\ELearning\ELearning\App_Data\ELearning.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sql = "SELECT NiveauEtude,Nom FROM studentTable WHERE id="+userId+";";
                SqlCommand myComm = new SqlCommand(sql, myCon);

                SqlDataReader myReader = myComm.ExecuteReader();
                if (myReader.Read())
                {
                    niveauEtude = myReader["niveauEtude"].ToString().Trim();
                    ViewBag.Nom = myReader["Nom"].ToString().Trim();
                }
                myReader.Close();

                string sqlSelect = "SELECT * FROM CoursesTable WHERE niveauEtude='"+niveauEtude+"' ORDER BY DateDebut ASC;";
                SqlCommand myCommSelect = new SqlCommand(sqlSelect, myCon);

                SqlDataReader myReaderSelect = myCommSelect.ExecuteReader();
                while (myReaderSelect.Read())
                {
                    myCoursesList.Add(new Courses {
                        Id = Convert.ToInt32(myReaderSelect["Id"].ToString().Trim()),
                        Nom = myReaderSelect["Nom"].ToString().Trim(),
                        DateDebut = Convert.ToDateTime(myReaderSelect["DateDebut"].ToString().Trim()).ToLongDateString(),
                        Description = myReaderSelect["Description"].ToString().Trim(),
                        photo = myReaderSelect["photo"].ToString().Trim(),
                        NiveauEtude = myReaderSelect["NiveauEtude"].ToString().Trim(),
                    });
                }

                ViewBag.myCoursesList = myCoursesList;

                myReaderSelect.Close();


            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }

            return View("courses",myCoursesList);

        }
        //============================================













        public void RemplirSelect()
        {
            //List
            List<String> myNivEtudList = new List<String>();

            //connect to the database
            SqlConnection myCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\elmehdi\source\repos\ELearning\ELearning\App_Data\ELearning.mdf;Integrated Security=True");
            try
            {
                myCon.Open();

                string sql = "SELECT niveauEtude FROM niveauEtudeTable";
                SqlCommand myComm = new SqlCommand(sql, myCon);

                SqlDataReader myReader = myComm.ExecuteReader();
                while (myReader.Read())
                {
                    myNivEtudList.Add(myReader["niveauEtude"].ToString().Trim());
                }

                ViewBag.myNivEtudList = myNivEtudList;

            }
            catch (Exception ex)
            {
                //afficher l'erreur quelque part
            }
            finally
            {
                myCon.Close();
            }
        }


        public ActionResult email()
        {
            ViewBag.msgError = "";
            ViewBag.msgSucces = "";
            return View("email");
        }

        [HttpPost]
        public ActionResult email(Email email)
        {
            if (sendMail(email))
            {
                ViewBag.msgSucces = "l'email a ete envoyer avec succes";
                return View("email");
            }
            else
            {
                ViewBag.msgError = "Un probleme est survenue lors de l'envoie de l'email";
                return View("email");
            }   
        }


        public Boolean sendMail(Email email)
        {
            ///apres avoir mis ces information dans le AppSettings

            //recuperer dans appSetings mon email
            string sender = System.Configuration.ConfigurationManager.AppSettings["mailSender"];

            //recuperer dans appSetings mon password
            string pass = System.Configuration.ConfigurationManager.AppSettings["mailPass"];

            try
            {
                ///Vehicule : creer et configurer un client smtp=========================================

                //(nom du serveur,port)
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com", 587);

                //temps maximal d'envoie
                smtpClient.Timeout = 3000;

                //protocole pour securiser l'envoie
                smtpClient.EnableSsl = true;

                //Enumeration
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                //refuser les informations de connexion par default pour utiliser les miens
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(sender, pass);
                //======================================================================================

                ///Passager=============================================================================
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, email.To, email.Subject, email.Message);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                smtpClient.Send(mailMessage);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public Boolean sendMailAfterSignUp(string userName,string passUser,string emailSendTo)
        {
            ///apres avoir mis ces information dans le AppSettings

            //recuperer dans appSetings mon email
            string sender = System.Configuration.ConfigurationManager.AppSettings["mailSender"];

            //recuperer dans appSetings mon password
            string pass = System.Configuration.ConfigurationManager.AppSettings["mailPass"];

            try
            {
                ///Vehicule : creer et configurer un client smtp=========================================

                //(nom du serveur,port)
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

                //temps maximal d'envoie
                smtpClient.Timeout = 3000;

                //protocole pour securiser l'envoie
                smtpClient.EnableSsl = true;

                //Enumeration
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                //refuser les informations de connexion par default pour utiliser les miens
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(sender, pass);
                //======================================================================================

                ///Passager=============================================================================
                string messageToSend = "UserName : " + userName + "\nPassWord : " + passUser;
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage(sender, emailSendTo, "ELearning UserName & PassWord", messageToSend);
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                smtpClient.Send(mailMessage);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }




        // GET: Inscription
        public ActionResult courseDetail(int id)//
        {
            string y = "";

            //requete course  where id===============================================
            myDataBaseDataContext myDataBase = new myDataBaseDataContext();

            var resSelectWhereID = from obj in myDataBase.CoursesTables
                               where obj.Id == id
                               select obj;

            CoursesTable choosenCourse = resSelectWhereID.SingleOrDefault<CoursesTable>();

            ViewBag.choosenCourse = choosenCourse;
            //=====================================================================================



            //requete Video========================================================================
            var resStringVideo = from _CourseVideoJencts in myDataBase.CourseVideoJencts
                                 join _VideosTables in myDataBase.VideosTables
                                 on _CourseVideoJencts.IdVideo equals _VideosTables.IdVideo

                                 where _CourseVideoJencts.IdCourse == id && _VideosTables.statut=="public"
                                 select _VideosTables.urlFolder;

            string urlVideo = resStringVideo.SingleOrDefault<string>();

            GlobalVar.urlVideo = urlVideo;
            //=====================================================================================


            string x = "";

            return View("courseDetail");
        }



    }
}