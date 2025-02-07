using UnityEngine;

namespace ThisIsAnAttack.Controllers;

public class RealtimeScoreSubmissionSession
{
    private IPA.Logging.Logger Logger { get; }

    public RealtimeScoreSubmissionSession(
        IPA.Logging.Logger logger)
    {
        this.Logger = logger;
    }

    #region Note cut interceptor

    //[AffinityPatch(typeof(ScoreController), nameof(ScoreController.HandleNoteWasCut))]
    //[AffinityPostfix]
    //public void NoteWasCutPostfix(
    //    NoteController noteController,
    //    in NoteCutInfo noteCutInfo)
    //{
    //    if (noteController.noteData.colorType == ColorType.None)
    //    {
    //        this.GameProgress.BombsPassed.Increment();
    //        this.GameProgress.BombsCut.Increment();
    //    }
    //    else
    //    {
    //        this.GameProgress.NotesPassed.Increment();
    //        if (noteCutInfo.allIsOK)
    //        {
    //            this.GameProgress.NotesCut.Increment();
    //        }
    //        else
    //        {
    //            this.GameProgress.NotesBadCut.Increment();
    //            this.GameProgress.NotesMissed.Increment();
    //        }
    //    }

    //    throw new NotImplementedException();
    //}

    #endregion

    #region Note miss interceptor

    //[AffinityPatch(typeof(ScoreController), nameof(ScoreController.HandleNoteWasMissed))]
    //[AffinityPostfix]
    //public void NoteWasMissedPostfix(NoteController noteController)
    //{
    //    if (noteController.noteData.colorType == ColorType.None)
    //    {
    //        this.GameProgress.BombsPassed.Increment();
    //    }
    //    else
    //    {
    //        this.GameProgress.NotesPassed.Increment();
    //        this.GameProgress.NotesMissed.Increment();
    //    }

    //    var noteData = noteController.noteData;
    //    throw new NotImplementedException();
    //}

    #endregion

    public void Initialize(CancellationToken cancellationToken)
    {
        this.Logger.Debug("Initializing RealtimeScoreSubmissionSession.");

        // Pause controller
        var pauseController = Resources.FindObjectsOfTypeAll<PauseController>().FirstOrDefault();
    }
}
