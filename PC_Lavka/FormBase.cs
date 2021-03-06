﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleCSV;
using Models;
using System.Configuration;
using SqlManaging;
using Install;
using Views;

namespace PC_Lavka
{
  public partial class FormBase : Form
  {
    public User CurrentUser;

    public FormBase()
    {
      InitializeComponent();
      CurrentUser = new User();
    }

    private void StartLoad()
    {
      foreach (Vendor ven in Vendor.All())
      {
        byVendorToolStripMenuItem.DropDownItems.Add(ven.Name, null, byVendor_Click);
        byVendorToolStripMenuItem1.DropDownItems.Add(ven.Name, null, byVendor_Click);
      }

      foreach (Category cat in Category.All())
      {
        byCategoryToolStripMenuItem.DropDownItems.Add(cat.Name, null, byCategory_Click);
        byCategoryToolStripMenuItem1.DropDownItems.Add(cat.Name, null, byCategory_Click);
      }

      adminToolStripMenuItem.Enabled = CurrentUser.is_admin;

    }

    private void FormBase_Load(object sender, EventArgs e)
    {
      if (ConfigurationManager.AppSettings["Installed"] == "false")
      {
        FormInstall form = new FormInstall();
        if (form.ShowDialog() == DialogResult.OK)
        {
          Configuration config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
          config.AppSettings.Settings.Remove("Installed");
          config.AppSettings.Settings.Add("Installed", "true");
          config.Save(ConfigurationSaveMode.Minimal);
        }
        else
          this.Close();
      }
      SqlManager.ConnectToServer(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);
      UserLoginForm login = new UserLoginForm(this);
      if (login.ShowDialog() == DialogResult.OK)
      {
        userBoxCurrent.SetCurrentUser(CurrentUser);
        StartLoad();
      }
      else
      {
        this.Close();
      }

    }

    private void flpBase_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        ctmnBase.Show(flpBase, e.Location); ;
      }
    }

    private void allToolStripMenuItem_Click(object sender, EventArgs e)
    {
      GetAllProducts();
    }

    private void byCategory_Click(object sender, EventArgs e)
    {
      flpBase.Controls.Clear();

      foreach (Product prod in Product.FindByCategory((sender as ToolStripItem).Text))
      {
        ProductBox pb = new ProductBox(prod, CurrentUser.is_admin);
        pb.onDelete += this.OnProductBoxDelete;
        pb.onAddShoppingCart += pb_onAddShoppingCart;
        flpBase.Controls.Add(pb);
      }
    }

    private void byVendor_Click(object sender, EventArgs e)
    {
      flpBase.Controls.Clear();

      foreach (Product prod in Product.FindByVendor((sender as ToolStripItem).Text))
      {
        ProductBox pb = new ProductBox(prod, CurrentUser.is_admin);
        pb.onDelete += this.OnProductBoxDelete;
        pb.onAddShoppingCart += pb_onAddShoppingCart;
        flpBase.Controls.Add(pb);
      }
    }

    private void GetAllProducts()
    {
      flpBase.Controls.Clear();

      foreach (Product prod in Product.All())
      {
        ProductBox pb = new ProductBox(prod, CurrentUser.is_admin);
        pb.onDelete += this.OnProductBoxDelete;
        pb.onAddShoppingCart += pb_onAddShoppingCart;
        flpBase.Controls.Add(pb);
      }
    }

    void pb_onAddShoppingCart(object sender)
    {
      ProductBox pb = (sender as ProductBox).CloneObj();
      pb.onDelete += this.OnProductBoxDelete;
      flpShoppingCart.Controls.Add(pb);
      PriceAdd((sender as ProductBox).CurrentProduct.Price);
    }

    private void OnProductBoxDelete(object sender)
    {
      flpBase.Controls.Remove((sender as ProductBox));
      int old = flpShoppingCart.Controls.Count;
      flpShoppingCart.Controls.Remove((sender as ProductBox));
      if (flpShoppingCart.Controls.Count != old)
        PriceAdd(-(sender as ProductBox).CurrentProduct.Price);
    }

    private void clearToolStripMenuItem_Click(object sender, EventArgs e)
    {
      flpBase.Controls.Clear();
    }

    private void PriceAdd(int num)
    {
      int oldPrice = Convert.ToInt32(toolStripStatusLbPrice.Text);
      toolStripStatusLbPrice.Text = (oldPrice + num).ToString();
    }

    private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      flpBase.Controls.Clear();
    }

    private void allToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      GetAllProducts();
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void productToolStripMenuItem_Click(object sender, EventArgs e)
    {
      FormFor.ProductForm form = new FormFor.ProductForm();
      if (form.ShowDialog() == DialogResult.OK)
      {
        ProductBox pb = new ProductBox(form.NewProduct, CurrentUser.is_admin);
        pb.onDelete += this.OnProductBoxDelete;
        pb.onAddShoppingCart += pb_onAddShoppingCart;
        flpBase.Controls.Add(pb);
      }
    }

  }
}