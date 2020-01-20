using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Speech;
using Android.Widget;
using Android.Content;

namespace SpeechToText
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private bool isRecording;
        private readonly int VOICE = 10;
        private TextView textbox;
        private Button recButton;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            isRecording = false;
            //set out view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main); //get the resources from the layout
            recButton = FindViewById<Button>(Resource.Id.btnRecord);
            textbox = FindViewById<TextView>(Resource.Id.textYourText);

            //check to see if we can actually record - if we can, assign the event to the button
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no Mic, no rec. Disable the button and output an alert
                var alert = new Android.App.AlertDialog.Builder(recButton.Context);
                alert.SetTitle("You Don't seem to have a Mic or record with");
                alert.SetPositiveButton("OK", (sender, e) =>
                {
                    textbox.Text = "No Mic present";
                    recButton.Enabled = false;
                    return;
                });
                alert.Show();
            }
            else
            {
                recButton.Click += delegate
                {
                    // change the text on the button
                    recButton.Text = "End Recording";
                    isRecording = !isRecording;
                    if (isRecording)
                    {
                        //create the intent and start the activity
                        var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                        // put a message on the modal dialog
                        voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, Application.Context.GetString(Resource.String.messageSpeakNow));

                        //if there is more then 1.5s of silence, consider the speech over
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                        voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

                        // you can specify other languages recognised here, for example
                        //voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German;
                        // if you with it to recongnise the default locale language and german
                        // if you do use another locale, regional dialects may not be recognised very well

                        voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                        StartActivityForResult(voiceIntent, VOICE);

                    }
                };
            }

            // SetContentView(Resource.Layout.activity_main);
        }
        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)
        {
            if (requestCode == VOICE)
            {
                if(resultVal == Result.Ok)
                { 
                    var matches = data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string textInput = textbox.Text + matches[0];
                        // limit the output to 500 characters
                        if (textInput.Length > 500)
                        {
                            textInput = textInput.Substring(0, 500);
                            
                        }
                        textbox.Text = textInput;
                    }
                    else
                    {
                        textbox.Text = "No speech was recognised";
                        // change the text back on the button
                        
                    }
                    recButton.Text = "Start Recording";

                }
            }
                base.OnActivityResult(requestCode, resultVal, data);
        }        
    }
}