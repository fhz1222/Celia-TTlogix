using System.Linq;
using FluentValidation;
using TT.Services.Interfaces;
using TT.Services.Models;
using TT.Services.Services;

namespace TT.Controllers.Validators
{
    public class PartMasterValidator : AbstractValidator<PartMasterDto>
    {
        public PartMasterValidator(IUtilityService utilityService)
        {
            RuleFor(dto => dto.ProductCode1)
                .NotEmpty().WithMessage("ProductCodeCannotBeEmpty")
                .MaximumLength(30).WithMessage("ProductCodeCannotExceedChars");

            RuleFor(dto => dto.Description)
                .NotEmpty().WithMessage("DescriptionCannotBeEmpty");

            RuleFor(dto => dto.UOM)
                .NotEmpty().WithMessage("UOMCannotBeEmpty");

            RuleFor(dto => dto.OriginCountry)
                .NotEmpty().WithMessage("OriginCountryCannotBeEmpty");

            RuleFor(dto => dto.PackageType)
                .NotEmpty().WithMessage("PackageTypeCannotBeEmpty");

            RuleFor(dto => dto.SPQ)
                .GreaterThan(0)
                .When(dto => dto.IsStandardPackaging)
                .WithMessage("SPQMustBeGreaterThan0");

            RuleFor(dto => dto.OrderLot)
                .GreaterThan(0).WithMessage("OrderLotMustBeGreaterThan0");

            RuleFor(dto => dto.Length)
                .GreaterThan(0).WithMessage("LengthMustBeGreaterThan0");

            RuleFor(dto => dto.Width)
                .GreaterThan(0).WithMessage("WidthMustBeGreaterThan0");

            RuleFor(dto => dto.Height)
                .GreaterThan(0).WithMessage("HeightMustBeGreaterThan0");

            RuleFor(dto => dto.NetWeight)
                .GreaterThan(0).WithMessage("NetWeightMustBeGreaterThan0");

            RuleFor(dto => dto.GrossWeight)
                .GreaterThan(0).WithMessage("GrossWeightMustBeGreaterThan0");

            RuleFor(dto => dto.SupplierID)
                .NotEmpty().WithMessage("SupplierIDCannotBeEmpty");

            RuleFor(dto => dto.CPartSPQ)
                .GreaterThan(0)
                .When(dto => dto.IsCPart)
                .WithMessage("CPartSPQMustBeGreaterThan0");

            RuleFor(dto => dto.iLogReadinessStatus)
                .Must(status => UtilityService.ILOG_READINESS_STATUSES.Contains(status))
                .WithMessage("iLogReadinessStatusMustBeOneOfDefined");

            RuleFor(dto => dto.IsMixed)
                .NotEqual(true)
                .When(dto => dto.IsCPart)
                .WithMessage("CPartCantBeMixed");

            RuleFor(dto => dto.PalletTypeId)
                .Must(id => utilityService.ValidatePalletTypeId(id))
                .WithMessage("PalletTypeIdMustBeOneOfDefined");

            RuleFor(dto => dto.ELLISPalletTypeId)
                .Must(id => utilityService.ValidateELLISPalletTypeId(id))
                .WithMessage("ELLISPalletTypeIdMustBeOneOfDefined");

            RuleFor(dto => dto.UnloadingPointId)
                .Must((dto, id) => utilityService.ValidateUnloadingPointId(id, dto.CustomerCode))
                .WithMessage("UnloadingPointDoesNotExist");

            RuleFor(dto => dto.LengthInternal)
                .GreaterThan(0).WithMessage("LengthTTMustBePositive");
            RuleFor(dto => dto.WidthInternal)
                .GreaterThan(0).WithMessage("WidthTTMustBePositive");
            RuleFor(dto => dto.HeightInternal)
                .GreaterThan(0).WithMessage("HeightTTMustBePositive");
            RuleFor(dto => dto.NetWeightInternal)
                .GreaterThan(0).WithMessage("NetWeightTTMustBePositive");
            RuleFor(dto => dto.GrossWeightInternal)
                .GreaterThan(0).WithMessage("GrossWeightTTMustBePositive");
        }
    }
}
