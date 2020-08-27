﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Data;
using Envelopes.Helpers;
using Envelopes.Pages.BudgetPage.CategoriesGrid;
using Moq;
using NUnit.Framework;

namespace Tests.Envelopes.Pages.BudgetPage.CategoriesGrid {
    [Apartment(ApartmentState.STA)]
    class CategoriesGridPresenterTests {
        private CategoriesGridPresenter categoriesGridPresenter;
        private Mock<ICategoriesGridViewModel> categoriesGridViewModelMock;
        private Mock<IDataService> dataServiceMock;
        private Mock<IGridValidator> gridValidatorMock;
        private Mock<ICategoriesGridView> categoriesGridView;



        [SetUp]
        public void Setup() {
            categoriesGridViewModelMock = new Mock<ICategoriesGridViewModel>();
            dataServiceMock = new Mock<IDataService>();
            gridValidatorMock = new Mock<IGridValidator>();
            categoriesGridView = new Mock<ICategoriesGridView>();

            categoriesGridPresenter = new CategoriesGridPresenter(categoriesGridView.Object, categoriesGridViewModelMock.Object, dataServiceMock.Object, gridValidatorMock.Object);
        }

        [Test]
        public void ParseAmountFromString_RegularNumberIsNotChanged() {
            var dataGrid = new DataGrid();
            var dataGridColumn = new DataGridTextColumn();
            var dataGridRow = new DataGridRow();
            var element = new FrameworkElement();

            categoriesGridView.Setup(mock => mock.CategoriesDataGrid ).Returns(dataGrid);
            categoriesGridView.Raise(mock => mock.DataGridCellEditEnding += null, new DataGridCellEditEndingEventArgs(dataGridColumn, dataGridRow, element, DataGridEditAction.Commit));
        }

        [Test]
        public void ParseAmountFromString_SubtractionWorks() {
   
        }

        [Test]
        public void ParseAmountFromString_AdditionWorks() {
            
        }


    }
}