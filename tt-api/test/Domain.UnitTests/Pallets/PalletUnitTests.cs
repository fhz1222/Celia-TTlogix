using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UnitTests.Pallets
{
    public class PalletUnitTests
    {

        private const string CustomerCode1 = "CUST 1";
        private const string CustomerCode2 = "CUST 2";
        private const string WHCode1 = "WH 1";
        private const string WHCode2 = "WH 2";


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PutawayPalletCheckIfCanBeAdjusted()
        {
            var putawayPallet = new Pallet()
            {
                Id = "TESA000001",
                WhsCode = WHCode1,
                Status = StorageStatus.Putaway,
                Product = new Product() 
                { 
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode1,
                    }
                }
            };

            Assert.True(putawayPallet.CanBeAdjusted(WHCode1, CustomerCode1, InventoryAdjustmentJobType.Normal));

            Assert.Throws<IncorrectPalletException>(() => putawayPallet.CanBeAdjusted(WHCode2, CustomerCode2, InventoryAdjustmentJobType.Normal));
            
            Assert.Throws<IncorrectPalletException>(() => putawayPallet.CanBeAdjusted(WHCode1, CustomerCode2, InventoryAdjustmentJobType.UndoZeroOut));

            Assert.Throws<IncorrectStorageDetailStatusException>(() => putawayPallet.CanBeAdjusted(WHCode1, CustomerCode1, InventoryAdjustmentJobType.UndoZeroOut));

            
        }

        [Test]
        public void CheckIfQuarantinePalletCanBeAdjusted()
        {
            var quarantinePallet = new Pallet()
            {
                Id = "TESA000001",
                WhsCode = WHCode2,
                Status = StorageStatus.Quarantine,
                Product = new Product()
                {
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode2,
                    }
                }
            };

            Assert.Throws<IncorrectPalletException>(() => quarantinePallet.CanBeAdjusted(WHCode1, CustomerCode1, InventoryAdjustmentJobType.Normal));

            Assert.True(quarantinePallet.CanBeAdjusted(WHCode2, CustomerCode2, InventoryAdjustmentJobType.Normal));

            Assert.Throws<IncorrectPalletException>(() => quarantinePallet.CanBeAdjusted(WHCode1, CustomerCode1, InventoryAdjustmentJobType.UndoZeroOut));

            Assert.Throws<IncorrectStorageDetailStatusException>(() => quarantinePallet.CanBeAdjusted(WHCode2, CustomerCode2, InventoryAdjustmentJobType.UndoZeroOut));

        }

        [Test]
        public void CheckIfPickedPalletCanBeAdjusted()
        {
            var picketPallet = new Pallet()
            {
                Id = "TESA000003",
                WhsCode = WHCode1,
                Status = StorageStatus.Picked,
                Product = new Product()
                {
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode1,
                    }
                }
            };

            Assert.Throws<IncorrectStorageDetailStatusException>(() => picketPallet.CanBeAdjusted(WHCode1, CustomerCode1, InventoryAdjustmentJobType.Normal));
        }

        [Test]
        public void ZeroOutPalletCheckIfCanBeAdjusted()
        {
            var zeroOutPallet = new Pallet()
            {
                Id = "TESA000003",
                WhsCode = WHCode1,
                Status = StorageStatus.ZeroOut,
                Product = new Product()
                {
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode1,
                    }
                }
            };

            Assert.Throws<IncorrectStorageDetailStatusException>(() => zeroOutPallet.CanBeAdjusted(WHCode1, CustomerCode1, InventoryAdjustmentJobType.Normal));
            Assert.True(zeroOutPallet.CanBeAdjusted(WHCode1, CustomerCode1, InventoryAdjustmentJobType.UndoZeroOut));
            Assert.Throws<IncorrectPalletException>(() => zeroOutPallet.CanBeAdjusted(WHCode2, CustomerCode2, InventoryAdjustmentJobType.UndoZeroOut));
        }

        [Test]
        public void IsInQuarantineTest()
        {
            var quarantinePallet = new Pallet()
            {
                Id = "TESA000003",
                WhsCode = WHCode1,
                Status = StorageStatus.Quarantine,
                Product = new Product()
                {
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode1,
                    }
                }
            };
            var putawayPallet = new Pallet()
            {
                Id = "TESA000001",
                WhsCode = WHCode1,
                Status = StorageStatus.Putaway,
                Product = new Product()
                {
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode1,
                    }
                }
            };

            Assert.IsTrue(quarantinePallet.IsInQuarantine);
            Assert.IsFalse(putawayPallet.IsInQuarantine);
        }

        [Test]
        public void CheckIfPalletsCanBeDecanted()
        {
            var quarantinePallet = new Pallet()
            {
                Id = "TESA000001",
                WhsCode = WHCode2,
                Status = StorageStatus.Quarantine,
                Product = new Product()
                {
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode2,
                    }
                }
            };

            var putawayPallet = new Pallet()
            {
                Id = "TESA000002",
                WhsCode = WHCode1,
                Status = StorageStatus.Putaway,
                Product = new Product()
                {
                    Code = "ABC",
                    CustomerSupplier = new CustomerSupplier()
                    {
                        CustomerCode = CustomerCode1,
                    }
                }
            };

            Assert.Throws<IncorrectStorageDetailStatusException>(() => quarantinePallet.CanBeDecanted(WHCode2, CustomerCode2));

            Assert.Throws<IncorrectPalletException>(() => putawayPallet.CanBeDecanted(WHCode2, CustomerCode1));

            Assert.Throws<IncorrectPalletException>(() => putawayPallet.CanBeDecanted(WHCode1, CustomerCode2));

            Assert.True(putawayPallet.CanBeDecanted(WHCode1, CustomerCode1));

        }
    }
}
