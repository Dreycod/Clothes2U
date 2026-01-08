using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Marque;
using Shared.DTO.Mesures;
using Shared.DTO.Taille;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace FrontBlazor.Services;

public class TailleWebService : CaracteristiqueService<TailleDTO>, ITailleService
{
    public TailleWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task PutTailleMesuresAsync(int tailleId, List<MesureDTO> MesuresDTO)
    {

        var body = JsonContent.Create(MesuresDTO);
        var response = await PutWithCredentialsAsync($"{_httpClient.BaseAddress}{"Taille"}/id/{tailleId}/mesures", body);
        response.EnsureSuccessStatusCode();
    }
}