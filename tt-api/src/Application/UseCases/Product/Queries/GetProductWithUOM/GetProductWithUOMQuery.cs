using MediatR;
using Application.Interfaces.Repositories;

namespace Application.UseCases.Product.Queries.GetProductWithUOM;

public class GetProductWithUOMQuery : IRequest<IEnumerable<ProductWithUOMDto>>
{
    public ProductWithUOMDtoFilter filter { get; set; } = null!;
}

public class GetProductWithUOMQueryHandler : IRequestHandler<GetProductWithUOMQuery, IEnumerable<ProductWithUOMDto>>
{
    private readonly IProductRepository productRepository;

    public GetProductWithUOMQueryHandler(IProductRepository productRepository)
    {
        this.productRepository = productRepository;
    }

    public Task<IEnumerable<ProductWithUOMDto>> Handle(GetProductWithUOMQuery request, CancellationToken cancellationToken)
    {
        var result = productRepository.GetProductsWithUOM(request.filter);
        return Task.FromResult(result);
    }
}
