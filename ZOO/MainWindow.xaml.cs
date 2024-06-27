using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ZOO
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["ZOO.Properties.Settings.PractSqlConnectionString"].ConnectionString;

            sqlConnection = new SqlConnection(connectionString);

            MuestraZoos();
            MuestraAnimales();
        }
      private void MuestraZoos()
        {
            try
            {
                string consulta = "select * from ZOO";

                SqlDataAdapter sqlDataadapter = new SqlDataAdapter(consulta, sqlConnection);

                using (sqlDataadapter)
                {
                    DataTable zooTabla = new DataTable();
                    sqlDataadapter.Fill(zooTabla);

                    //aqui ponemos lo  q queremos que se muestre//
                    ListasZoos.DisplayMemberPath = "Ubicacion";
                    ListasZoos.SelectedValuePath = "Id";
                    ListasZoos.ItemsSource = zooTabla.DefaultView;
                }
            }
            catch(Exception e)
            { 
                MessageBox.Show(e.ToString());
            
            }
          
        }
        private void MuestraAnimales()
        {

            try
            {
                string consulta = "select * from Animales";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable tablaAnimales = new DataTable();
                    sqlDataAdapter.Fill(tablaAnimales);

                    ListaAnimales.DisplayMemberPath = "Animal";
                    ListaAnimales.SelectedValuePath = "Id";
                    ListaAnimales.ItemsSource = tablaAnimales.DefaultView;
                }
            }
            catch (Exception e)
            {
               MessageBox.Show(e.ToString());
            }
         
        }
        private void MuestraAnimalesAsociados()
        {
            try
            {
                string consulta = "select * from Animales a Inner join AnimalZoo az on a.Id = az.AnimalesId where az.ZooId = @ZooId";


                SqlCommand sqlCommand = new SqlCommand( consulta, sqlConnection);

                SqlDataAdapter sqlDataadapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataadapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ZooId", ListasZoos.SelectedValue);

                    DataTable tablaAnimal = new DataTable();
                    sqlDataadapter.Fill(tablaAnimal);

                    //aqui ponemos lo  q queremos que se muestre//
                   listaAnimalesAsociados.DisplayMemberPath = "Animal";
                   listaAnimalesAsociados.SelectedValuePath = "Id";
                   listaAnimalesAsociados.ItemsSource = tablaAnimal.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }

        }
        
        private void MuestraZooElegidoEnTextBox()
        {
            try
            {
                string consulta = "select Ubicacion from ZOO where Id = @ZOOId";

                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                SqlDataAdapter sqlDataadapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataadapter)
                {
                    sqlCommand.Parameters.AddWithValue("@ZOOId", ListasZoos.SelectedValue);
                    DataTable tablaAnimalZoo = new DataTable();
                    sqlDataadapter.Fill(tablaAnimalZoo);

                    miTextBox.Text = tablaAnimalZoo.Rows[0]["Ubicacion"].ToString();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
        }

        private void MuestraAnimalElegidoEnTextBox()
        {
            try
            {
                string consulta = "select Animal from Animales where Id = @AnimalesId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {
                    sqlCommand.Parameters.AddWithValue("@AnimalesId", ListaAnimales.SelectedValue);
                    DataTable tablaAnimalesZoo = new DataTable();
                    sqlDataAdapter.Fill(tablaAnimalesZoo);

                    miTextBox.Text = tablaAnimalesZoo.Rows[0]["Animal"].ToString();
                }
            }
            catch ( Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void ListaAnimales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraAnimalElegidoEnTextBox();
        }
        private void ListasZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MuestraAnimalesAsociados();
            MuestraZooElegidoEnTextBox();
        }
        private void EliminarZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Delete from ZOO where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListasZoos.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZoos();
              
            }
        }

        private void AgregarAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Insert into ZOO values (@Ubicacion)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Ubicacion", miTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZoos();

            }
        }

        private void AgregarAnimalaZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Insert into AnimalZoo values (@ZooId, @AnimalesId)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListasZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalesId", ListaAnimales.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimalesAsociados();
            }
        }
        private void AgregarNuevoAnimalalZoo_Click( object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Insert into Animales values (@AnimalesId)";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalesId", miTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimales();

            }
        }
        
        private void EliminarAnimadelZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Delete from Animales where id = @AnimalesId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalesId", ListaAnimales.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
               MuestraAnimales();

            }
        }

        private void ActualizarZoo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Update ZOO Set Ubicacion = @Ubicacion where Id= @ZOOId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZOOId", ListasZoos.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Ubicacion", miTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraZoos();
            }
        }

        private void ActualizarAnimal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string consulta = "Update Animales Set Animal = @Animal where Id= @AnimalesId";
                SqlCommand sqlCommand = new SqlCommand(consulta, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("AnimalesId",ListaAnimales.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Animal", miTextBox.Text);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
                MuestraAnimales();
            }
        }
    }
  
}

