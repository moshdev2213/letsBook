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

import com.example.letsbook.FormData.SignInForm;
import com.example.letsbook.R;
import com.example.letsbook.Validation.ValidationResult;

public class SignIn extends AppCompatActivity {
    private EditText etSignInEmail;
    private EditText etSignInPassword;
    private CardView cvSignInBtn;
    private TextView tvredirectToSignUp;
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
                loginValidator(etSignInEmail.getText().toString(),etSignInPassword.getText().toString());
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
        int count = 0; // Assuming you have declared and initialized 'count' earlier

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

}