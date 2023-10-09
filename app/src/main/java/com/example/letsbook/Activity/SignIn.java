package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;
import androidx.core.splashscreen.SplashScreen;

import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.example.letsbook.ApiRoutes.AuthApi;
import com.example.letsbook.DialogAlerts.ProgressLoader;
import com.example.letsbook.FormData.SignInForm;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.AuthPassEmail;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;
import com.example.letsbook.Validation.ValidationResult;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SignIn extends AppCompatActivity {
    int count = 0;
    private EditText etSignInEmail;
    private EditText etSignInPassword;
    private CardView cvSignInBtn;
    private TextView tvredirectToSignUp;
    private ProgressLoader progressLoader;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        try {
            Thread.sleep(2000);
        } catch (InterruptedException e) {
            throw new RuntimeException(e);
        }
        SplashScreen splashScreen = SplashScreen.installSplashScreen(this);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            getSplashScreen();
        }
        setContentView(R.layout.activity_sign_in);

        etSignInPassword = findViewById(R.id.etSignInPassword);
        etSignInEmail = findViewById(R.id.etSignInEmail);
        tvredirectToSignUp = findViewById(R.id.tvredirectToSignUp);
        cvSignInBtn = findViewById(R.id.cvSignInBtn);

        cvSignInBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                authEmail(etSignInEmail.getText().toString(),etSignInPassword.getText().toString());
            }
        });
        tvredirectToSignUp.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(getApplicationContext(), SignUp.class));
            }
        });

    }
    private void loginValidator(String email, String password) {
        SignInForm userLoginForm = new SignInForm(email, password);
        ValidationResult emailValidation = userLoginForm.validateEmail(email);
        ValidationResult passwordValidation = userLoginForm.validatePassword(password);


        if (emailValidation instanceof ValidationResult.Valid) {
            count++;
        } else if (emailValidation instanceof ValidationResult.Invalid) {
            etSignInEmail.setError(((ValidationResult.Invalid) emailValidation).getErrorMsg());
        } else if (emailValidation instanceof ValidationResult.Empty) {
            etSignInEmail.setError(((ValidationResult.Empty) emailValidation).getErrorMsg());
        }

        if (passwordValidation instanceof ValidationResult.Valid) {
            count++;
        } else if (passwordValidation instanceof ValidationResult.Invalid) {
            etSignInPassword.setError(((ValidationResult.Invalid) passwordValidation).getErrorMsg());
        } else if (passwordValidation instanceof ValidationResult.Empty) {
            etSignInPassword.setError(((ValidationResult.Empty) passwordValidation).getErrorMsg());
        }
    }
    private void authEmail(String email, String password) {
        loginValidator(email, password);

        if (count == 2) {
            progressLoader = new ProgressLoader(this, "Verifying Login", "Please Wait");
            progressLoader.startProgressLoader();

            RetrofitService retrofitService = new RetrofitService();
            AuthApi authService = retrofitService.getRetrofit().create(AuthApi.class);

            Call<UserRecord> call = authService.getUserAuth(new AuthPassEmail(email, password));
            call.enqueue(new Callback<UserRecord>() {
                @Override
                public void onResponse(Call<UserRecord> call, Response<UserRecord> response) {
                    if (response.isSuccessful()) {
                        UserRecord user = response.body();
                        System.out.println("this is it SignIn: "+user);
                        if (user != null) {
                            Intent intent = new Intent(SignIn.this, Home.class);
                            intent.putExtra("user", user); // Assuming "user" is Parcelable or Serializable
                            startActivity(intent);
                            progressLoader.dismissProgressLoader();
                            finish();
                        }
                    } else {
                        Toast.makeText(SignIn.this, "Invalid Credentials", Toast.LENGTH_SHORT).show();
                        progressLoader.dismissProgressLoader();
                    }
                }

                @Override
                public void onFailure(Call<UserRecord> call, Throwable t) {
                    Toast.makeText(SignIn.this, "Server Error", Toast.LENGTH_SHORT).show();
                    progressLoader.dismissProgressLoader();
                }
            });
            count = 0;
        }
        count = 0;
    }


}