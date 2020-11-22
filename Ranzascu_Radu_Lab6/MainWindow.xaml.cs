using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AutoLotModel;

namespace Ranzascu_Radu_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }

    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource inventoryViewSource;
        CollectionViewSource customerOrdersViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Customers.Load();
            ctx.Inventories.Load();
            ctx.Orders.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            BindDataGrid();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty, firstNameValidationBinding);

            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty, lastNameValidationBinding);
        }

        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals cust.CustId
                             join inv in ctx.Inventories on ord.CarId equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem entitatea Customer
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
            }
        }

        private void btnSaveInv_Click(object sender, RoutedEventArgs e)
        {
            Inventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem entitatea Customer
                    inventory = new Inventory()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Inventories.Add(inventory);
                    inventoryViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    inventory.Make = makeTextBox.Text.Trim();
                    inventory.Color = colorTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                inventoryViewSource.View.MoveCurrentTo(inventory);

                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    inventory = (Inventory)inventoryDataGrid.SelectedItem;
                    ctx.Inventories.Remove(inventory);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                inventoryViewSource.View.Refresh();

                btnNewInv.IsEnabled = true;
                btnEditInv.IsEnabled = true;
                btnDeleteInv.IsEnabled = true;
                btnSaveInv.IsEnabled = false;
                btnCancelInv.IsEnabled = false;
                btnPrevInv.IsEnabled = true;
                btnNextInv.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
            }
        }

        private void btnSaveOrd_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem entitatea Customer
                    order = new Order()
                    {
                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnDeleteOrd.IsEnabled = true;
                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;
                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;
            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);

                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnDeleteOrd.IsEnabled = true;
                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;
                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNewOrd.IsEnabled = true;
                btnEditOrd.IsEnabled = true;
                btnDeleteOrd.IsEnabled = true;
                btnSaveOrd.IsEnabled = false;
                btnCancelOrd.IsEnabled = false;
                btnPrevOrd.IsEnabled = true;
                btnNextOrd.IsEnabled = true;
            }
            SetValidationBinding();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }

        private void btnNewInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
        }

        private void btnNewOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;

            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;
            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;
            btnPrevOrd.IsEnabled = false;
            btnNextOrd.IsEnabled = false;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            SetValidationBinding();
        }

        private void btnEditInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
        }

        private void btnEditOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;
            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;
            btnPrevOrd.IsEnabled = false;
            btnNextOrd.IsEnabled = false;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
        }

        private void btnDeleteInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNewInv.IsEnabled = false;
            btnEditInv.IsEnabled = false;
            btnDeleteInv.IsEnabled = false;
            btnSaveInv.IsEnabled = true;
            btnCancelInv.IsEnabled = true;
            btnPrevInv.IsEnabled = false;
            btnNextInv.IsEnabled = false;
            inventoryDataGrid.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
        }

        private void btnDeleteOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            btnNewOrd.IsEnabled = false;
            btnEditOrd.IsEnabled = false;
            btnDeleteOrd.IsEnabled = false;
            btnSaveOrd.IsEnabled = true;
            btnCancelOrd.IsEnabled = true;
            btnPrevOrd.IsEnabled = false;
            btnNextOrd.IsEnabled = false;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            customerDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
        }

        private void btnCancelInv_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewInv.IsEnabled = true;
            btnEditInv.IsEnabled = true;
            btnDeleteInv.IsEnabled = true;
            btnSaveInv.IsEnabled = false;
            btnCancelInv.IsEnabled = false;
            btnPrevInv.IsEnabled = true;
            btnNextInv.IsEnabled = true;
            inventoryDataGrid.IsEnabled = true;
            makeTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
        }

        private void btnCancelOrd_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNewOrd.IsEnabled = true;
            btnEditOrd.IsEnabled = true;
            btnDeleteOrd.IsEnabled = true;
            btnSaveOrd.IsEnabled = false;
            btnCancelOrd.IsEnabled = false;
            btnPrevOrd.IsEnabled = true;
            btnNextOrd.IsEnabled = true;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnPrevInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToPrevious();
        }

        private void btnPrevOrd_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }

        private void btnNextInv_Click(object sender, RoutedEventArgs e)
        {
            inventoryViewSource.View.MoveCurrentToNext();
        }

        private void btnNextOrd_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }

        private void btnNextOrd_Clikc(object sender, RoutedEventArgs e)
        {

        }
    }
}
