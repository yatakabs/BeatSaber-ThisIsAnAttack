//using Microsoft.Extensions.Logging;
//using ThisIsAnAttack.Remoing;
//using UnityEngine;

//namespace ThisIsAnAttack.Controllers;

/// <summary>
/// Unity controller for realtime score submission, which is used to submit scores to the server in real time.
/// This contorller is instanciated at every GameCore scene load, and is responsible for submitting scores to the server in real time.
///
/// </summary>
/// <remarks>
/// One instance is only responsible for exactly one map play session.
/// Therefore, this controller is instanciated at every GameCore scene load and destroyed at the end of the map play session.
/// Also, thus, the controller is designed to be immutable.
/// </remarks>
//public class RealtimeScoreSubmissionController : MonoBehaviour
//{
//    private IRealtimeScoreSubmitter RealtimeScoreSubmitter { get; }
//    private ILogger<RealtimeScoreSubmissionController> Logger { get; }

//    public RealtimeScoreSubmissionController(
//        IRealtimeScoreSubmitter realtimeScoreSubmitter,
//        ILogger<RealtimeScoreSubmissionController> logger)
//    {
//        this.RealtimeScoreSubmitter = realtimeScoreSubmitter;
//        this.Logger = logger;
//    }
//}
