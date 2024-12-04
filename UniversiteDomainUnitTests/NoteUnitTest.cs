using Moq;
using NUnit.Framework;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using System.Linq.Expressions;

namespace UniversiteDomainUnitTests;

public class CreateNoteUseCaseTests
{
    private Mock<IRepositoryFactory> _mockFactory;
    private Mock<INoteRepository> _mockNoteRepository;
    private Mock<IEtudiantRepository> _mockEtudiantRepository;
    private Mock<IUeRepository> _mockUeRepository;

    [SetUp]
    public void Setup()
    {
        _mockFactory = new Mock<IRepositoryFactory>();
        _mockNoteRepository = new Mock<INoteRepository>();
        _mockEtudiantRepository = new Mock<IEtudiantRepository>();
        _mockUeRepository = new Mock<IUeRepository>();

        _mockFactory.Setup(f => f.NoteRepository()).Returns(_mockNoteRepository.Object);
        _mockFactory.Setup(f => f.EtudiantRepository()).Returns(_mockEtudiantRepository.Object);
        _mockFactory.Setup(f => f.UeRepository()).Returns(_mockUeRepository.Object);
    }

    [Test]
    public async Task CreateNoteUseCase_ShouldCreateSuccessfully()
    {
        // Données de test
        long etudiantId = 1;
        long ueId = 2;
        float valeur = 15.0f;

        // Configuration des mocks
        _mockEtudiantRepository
            .Setup(repo => repo.FindAsync(etudiantId))
            .ReturnsAsync(new Etudiant
            {
                Id = etudiantId,
                ParcoursSuivi = new Parcours
                {
                    UesEnseignees = new List<Ue> { new Ue { Id = ueId } }
                }
            });

        _mockUeRepository
            .Setup(repo => repo.FindAsync(ueId))
            .ReturnsAsync(new Ue { Id = ueId });

        _mockNoteRepository
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>());

        _mockNoteRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Note>()))
            .ReturnsAsync(new Note
            {
                EtudiantId = etudiantId,
                UeId = ueId,
                Valeur = valeur
            });

        _mockNoteRepository
            .Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Création du UseCase
        var useCase = new CreateNoteUseCase(_mockFactory.Object);

        // Appel de la méthode
        var result = await useCase.ExecuteAsync(etudiantId, ueId, valeur);

        // Vérification
        Assert.NotNull(result);
        Assert.AreEqual(etudiantId, result.EtudiantId);
        Assert.AreEqual(ueId, result.UeId);
        Assert.AreEqual(valeur, result.Valeur);
    }


    [Test]
    public void CreateNoteUseCase_ShouldThrow_InvalidNoteValueException()
    {
        // Données de test
        long etudiantId = 1;
        long ueId = 2;
        float valeur = 25.0f; // Valeur invalide (> 20)

        var useCase = new CreateNoteUseCase(_mockFactory.Object);

        // Vérification de l'exception
        Assert.ThrowsAsync<InvalidNoteValueException>(() => useCase.ExecuteAsync(etudiantId, ueId, valeur));
    }

    [Test]
    public void CreateNoteUseCase_ShouldThrow_EtudiantNotFoundException()
    {
        // Données de test
        long etudiantId = 1;
        long ueId = 2;
        float valeur = 15.0f;

        // Mock : Étudiant non trouvé
        _mockEtudiantRepository.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync((Etudiant)null);

        var useCase = new CreateNoteUseCase(_mockFactory.Object);

        // Vérification de l'exception
        Assert.ThrowsAsync<EtudiantNotFoundException>(() => useCase.ExecuteAsync(etudiantId, ueId, valeur));
    }

    [Test]
    public void CreateNoteUseCase_ShouldThrow_UeNotFoundException()
    {
        // Données de test
        long etudiantId = 1;
        long ueId = 2;
        float valeur = 15.0f;

        var etudiant = new Etudiant { Id = etudiantId };

        // Mock : Étudiant trouvé, mais UE non trouvée
        _mockEtudiantRepository.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(etudiant);
        _mockUeRepository.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync((Ue)null);

        var useCase = new CreateNoteUseCase(_mockFactory.Object);

        // Vérification de l'exception
        Assert.ThrowsAsync<UeNotFoundException>(() => useCase.ExecuteAsync(etudiantId, ueId, valeur));
    }

    [Test]
    public void CreateNoteUseCase_ShouldThrow_DuplicateNoteException()
    {
        // Données de test
        long etudiantId = 1;
        long ueId = 2;
        float valeur = 15.0f;

        var existingNotes = new List<Note> { new Note { EtudiantId = etudiantId, UeId = ueId } };

        // Mock : Note déjà existante
        _mockEtudiantRepository.Setup(repo => repo.FindAsync(etudiantId)).ReturnsAsync(new Etudiant
        {
            Id = etudiantId,
            ParcoursSuivi = new Parcours
            {
                UesEnseignees = new List<Ue> { new Ue { Id = ueId } }
            }
        });

        _mockUeRepository.Setup(repo => repo.FindAsync(ueId)).ReturnsAsync(new Ue { Id = ueId });

        _mockNoteRepository
            .Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(existingNotes);

        var useCase = new CreateNoteUseCase(_mockFactory.Object);

        // Vérification de l'exception
        Assert.ThrowsAsync<DuplicateNoteException>(() => useCase.ExecuteAsync(etudiantId, ueId, valeur));
    }

}
