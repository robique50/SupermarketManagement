﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Supermarket.Models.BusinessLogic;
using Supermarket.Models.EntityLayer;
using Supermarket.Commands;

namespace Supermarket.ViewModels
{
    public class ProductVM : BasePropertyChanged
    {
        private ProductBLL productBLL = new ProductBLL();
        private CategoryBLL categoryBLL = new CategoryBLL();
        private ManufacturerBLL manufacturerBLL = new ManufacturerBLL();

        private Product selectedProduct;
        private string newProductName;
        private string newBarcode;
        private string newCategoryName;
        private string newManufacturerName;
        private Category newCategory;
        private Manufacturer newManufacturer;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Manufacturer> Manufacturers { get; set; }

        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }

        public ProductVM()
        {
            Products = new ObservableCollection<Product>(productBLL.GetAllProducts());
            Categories = new ObservableCollection<Category>(categoryBLL.GetAllCategories());
            Manufacturers = new ObservableCollection<Manufacturer>(manufacturerBLL.GetAllManufacturers());

            AddProductCommand = new RelayCommand<object>(AddProduct);
            EditProductCommand = new RelayCommand<object>(EditProduct);
            DeleteProductCommand = new RelayCommand<object>(DeleteProduct);
        }

        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                selectedProduct = value;
                NotifyPropertyChanged(nameof(SelectedProduct));
                if (selectedProduct != null)
                {
                    NewProductName = selectedProduct.ProductName;
                    NewBarcode = selectedProduct.Barcode;
                    NewCategory = Categories.FirstOrDefault(c => c.CategoryID == selectedProduct.CategoryID);
                    NewManufacturer = Manufacturers.FirstOrDefault(m => m.ManufacturerID == selectedProduct.ManufacturerID);
                    NewCategoryName = selectedProduct.CategoryName;
                    NewManufacturerName = selectedProduct.ManufacturerName;
                }
            }
        }

        public string NewProductName
        {
            get { return newProductName; }
            set
            {
                newProductName = value;
                NotifyPropertyChanged(nameof(NewProductName));
            }
        }

        public string NewBarcode
        {
            get { return newBarcode; }
            set
            {
                newBarcode = value;
                NotifyPropertyChanged(nameof(NewBarcode));
            }
        }

        public string NewCategoryName
        {
            get { return newCategoryName; }
            set
            {
                newCategoryName = value;
                NotifyPropertyChanged(nameof(NewCategoryName));
            }
        }

        public string NewManufacturerName
        {
            get { return newManufacturerName; }
            set
            {
                newManufacturerName = value;
                NotifyPropertyChanged(nameof(NewManufacturerName));
            }
        }

        public Category NewCategory
        {
            get { return newCategory; }
            set
            {
                newCategory = value;
                NotifyPropertyChanged(nameof(NewCategory));
            }
        }

        public Manufacturer NewManufacturer
        {
            get { return newManufacturer; }
            set
            {
                newManufacturer = value;
                NotifyPropertyChanged(nameof(NewManufacturer));
            }
        }

        public void RefreshProducts()
        {
            Products.Clear();
            foreach (var product in productBLL.GetAllProducts())
            {
                Products.Add(product);
            }

            Categories.Clear();
            foreach (var category in categoryBLL.GetAllCategories())
            {
                Categories.Add(category);
            }

            Manufacturers.Clear();
            foreach (var manufacturer in manufacturerBLL.GetAllManufacturers())
            {
                Manufacturers.Add(manufacturer);
            }
        }


        private void AddProduct(object parameter)
        {
            if (ValidateProductInputs(NewProductName, NewBarcode, NewCategoryName))
            {
                if (Products.Any(p => p.Barcode == NewBarcode))
                {
                    MessageBox.Show("Barcode must be unique.");
                    return;
                }

                if (NewCategory == null)
                {
                    var existingCategory = Categories.FirstOrDefault(c => c.CategoryName == NewCategoryName);
                    if (existingCategory == null)
                    {
                        NewCategory = new Category { CategoryName = NewCategoryName, IsActive = true };
                        categoryBLL.AddCategory(NewCategory);
                        Categories.Add(NewCategory);
                    }
                    else
                    {
                        NewCategory = existingCategory;
                    }
                }

                if (NewManufacturer == null)
                {
                    MessageBox.Show("Please select an existing manufacturer.");
                    return;
                }

                Product newProduct = new Product
                {
                    ProductName = NewProductName,
                    Barcode = NewBarcode,
                    CategoryID = NewCategory.CategoryID,
                    ManufacturerID = NewManufacturer.ManufacturerID,
                    CategoryName = NewCategory.CategoryName,
                    ManufacturerName = NewManufacturer.ManufacturerName,
                    IsActive = true
                };

                productBLL.AddProduct(newProduct);
                Products.Add(newProduct);
                ClearInputs();
            }
        }


        private void EditProduct(object parameter)
        {
            if (SelectedProduct != null && ValidateProductInputs(NewProductName, NewBarcode, NewCategoryName))
            {
                if (Products.Any(p => p.Barcode == NewBarcode && p.ProductID != SelectedProduct.ProductID))
                {
                    MessageBox.Show("Barcode must be unique.");
                    return;
                }

                if (NewCategory == null)
                {
                    var existingCategory = Categories.FirstOrDefault(c => c.CategoryName == NewCategoryName);
                    if (existingCategory == null)
                    {
                        NewCategory = new Category { CategoryName = NewCategoryName, IsActive = true };
                        categoryBLL.AddCategory(NewCategory);
                        Categories.Add(NewCategory);
                    }
                    else
                    {
                        NewCategory = existingCategory;
                    }
                }

                if (NewManufacturer == null)
                {
                    NewManufacturer = new Manufacturer { ManufacturerName = NewManufacturerName, IsActive = true };
                    manufacturerBLL.AddManufacturer(NewManufacturer);
                    Manufacturers.Add(NewManufacturer);
                }

                SelectedProduct.ProductName = NewProductName;
                SelectedProduct.Barcode = NewBarcode;
                SelectedProduct.CategoryID = NewCategory.CategoryID;
                SelectedProduct.ManufacturerID = NewManufacturer.ManufacturerID;
                SelectedProduct.CategoryName = NewCategory.CategoryName;  
                SelectedProduct.ManufacturerName = NewManufacturer.ManufacturerName;  

                productBLL.EditProduct(SelectedProduct);
                int index = Products.IndexOf(SelectedProduct);
                if (index >= 0)
                {
                    Products[index] = new Product
                    {
                        ProductID = SelectedProduct.ProductID,
                        ProductName = SelectedProduct.ProductName,
                        Barcode = SelectedProduct.Barcode,
                        CategoryID = SelectedProduct.CategoryID,
                        ManufacturerID = SelectedProduct.ManufacturerID,
                        IsActive = SelectedProduct.IsActive,
                        CategoryName = SelectedProduct.CategoryName,
                        ManufacturerName = SelectedProduct.ManufacturerName
                    };
                }
                ClearInputs();
            }
        }

        private void DeleteProduct(object parameter)
        {
            if (SelectedProduct != null)
            {
                productBLL.DeleteProduct(SelectedProduct.ProductID);
                Products.Remove(SelectedProduct);
                ClearInputs();
            }
        }

        private bool ValidateProductInputs(string productName, string barcode, string categoryName)
        {
            if (string.IsNullOrWhiteSpace(productName) || !Regex.IsMatch(productName, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Product name must be non-empty and contain only letters and spaces.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(barcode) || !Regex.IsMatch(barcode, @"^[0-9]+$"))
            {
                MessageBox.Show("Barcode must be non-empty and contain only digits.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(categoryName) || !Regex.IsMatch(categoryName, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Category name must be non-empty and contain only letters and spaces.");
                return false;
            }

            return true;
        }

        private void ClearInputs()
        {
            NewProductName = string.Empty;
            NewBarcode = string.Empty;
            NewCategory = null;
            NewManufacturer = null;
            NewCategoryName = string.Empty;
            NewManufacturerName = string.Empty;
            SelectedProduct = null;
        }
    }
}
