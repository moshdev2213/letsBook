package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.example.letsbook.ApiRoutes.AuthApi;
import com.example.letsbook.DialogAlerts.ProgressLoader;
import com.example.letsbook.FormData.SignUpform;
import com.example.letsbook.ModalDao.AuthSignUp;
import com.example.letsbook.ModalDao.AuthSignUpRes;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;
import com.example.letsbook.Validation.ValidationResult;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class SignUp extends AppCompatActivity {
    private EditText etNameSignUp;
    private EditText etEmailSignUp;
    private EditText etNicSignUp;
    private EditText etTelSignUp;
    private EditText etPasswordSignUp;
    private CardView cvSignUpBtn;
    private TextView tvRedirectLogin;
    private int count =0;
    private ProgressLoader progressLoader;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sign_up);

        etEmailSignUp = findViewById(R.id.etEmailSignUp);
        etNameSignUp = findViewById(R.id.etNameSignUp);
        etNicSignUp = findViewById(R.id.etNicSignUp);
        etTelSignUp = findViewById(R.id.etTelSignUp);
        etPasswordSignUp = findViewById(R.id.etPasswordSignUp);
        cvSignUpBtn = findViewById(R.id.cvSignUpBtn);
        tvRedirectLogin = findViewById(R.id.tvRedirectLogin);

        tvRedirectLogin.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                startActivity(new Intent(getApplicationContext(), SignIn.class));
            }
        });
        cvSignUpBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                authSignUp(
                        etTelSignUp.getText().toString(),
                        etEmailSignUp.getText().toString(),
                        etPasswordSignUp.getText().toString(),
                        etNicSignUp.getText().toString(),
                        etNameSignUp.getText().toString()
                );
            }
        });
    }
    private void validateSignUp(String tel, String email, String name, String nic,String password) {
        SignUpform userSignUp = new SignUpform(tel, email, name, nic,password);
        ValidationResult emailValidation = userSignUp.validateEmail(email);
        ValidationResult passwordValidation = userSignUp.validatePassword(password);
        ValidationResult nicValidation = userSignUp.validateNic(nic);
        ValidationResult telValidation = userSignUp.validateTelephone(tel);
        ValidationResult nameValidation = userSignUp.validateName(name);

        if (telValidation instanceof ValidationResult.Valid) {
            count++;
        } else if (telValidation instanceof ValidationResult.Invalid) {
            etTelSignUp.setError(((ValidationResult.Invalid) telValidation).getErrorMsg());
        } else if (telValidation instanceof ValidationResult.Empty) {
            etTelSignUp.setError(((ValidationResult.Empty) telValidation).getErrorMsg());
        }

        if (emailValidation instanceof ValidationResult.Valid) {
            count++;
        } else if (emailValidation instanceof ValidationResult.Invalid) {
            etEmailSignUp.setError(((ValidationResult.Invalid) emailValidation).getErrorMsg());
        } else if (emailValidation instanceof ValidationResult.Empty) {
            etEmailSignUp.setError(((ValidationResult.Empty) emailValidation).getErrorMsg());
        }

        if (nicValidation instanceof ValidationResult.Valid) {
            count++;
        } else if (nicValidation instanceof ValidationResult.Invalid) {
            etNicSignUp.setError(((ValidationResult.Invalid) nicValidation).getErrorMsg());
        } else if (nicValidation instanceof ValidationResult.Empty) {
            etNicSignUp.setError(((ValidationResult.Empty) nicValidation).getErrorMsg());
        }

        if (passwordValidation instanceof ValidationResult.Valid) {
            count++;
        } else if (passwordValidation instanceof ValidationResult.Invalid) {
            etPasswordSignUp.setError(((ValidationResult.Invalid) passwordValidation).getErrorMsg());
        } else if (passwordValidation instanceof ValidationResult.Empty) {
            etPasswordSignUp.setError(((ValidationResult.Empty) passwordValidation).getErrorMsg());
        }

        if (nameValidation instanceof ValidationResult.Valid) {
            count++;
        } else if (nameValidation instanceof ValidationResult.Invalid) {
            etNameSignUp.setError(((ValidationResult.Invalid) nameValidation).getErrorMsg());
        } else if (nameValidation instanceof ValidationResult.Empty) {
            etNameSignUp.setError(((ValidationResult.Empty) nameValidation).getErrorMsg());
        }
    }

    private void authSignUp(String tel, String email, String password, String nic,String name) {
        validateSignUp(tel, email, name, nic,password);
        System.out.println("helllnoo "+tel+email+name+nic+password);
        if (count == 5) {
            progressLoader = new ProgressLoader(this, "Verifying Register", "Please Wait");
            progressLoader.startProgressLoader();

            RetrofitService retrofitService = new RetrofitService();
            AuthApi authService = retrofitService.getRetrofit().create(AuthApi.class);

            Call<AuthSignUpRes> call = authService.createUserAuth(new AuthSignUp(tel, email, name, nic,password,password));
            call.enqueue(new Callback<AuthSignUpRes>() {
                @Override
                public void onResponse(Call<AuthSignUpRes> call, Response<AuthSignUpRes> response) {
                    System.out.println(response.body());
                    if (response.isSuccessful()) {
                        AuthSignUpRes user = response.body();
                        if (user != null) {
                            Intent intent = new Intent(SignUp.this, SignIn.class);
                            startActivity(intent);
                            progressLoader.dismissProgressLoader();
                            finish();
                        }
                    } else {
                        System.out.println("eror "+response.code());
                        Toast.makeText(SignUp.this, "Invalid data", Toast.LENGTH_SHORT).show();
                        progressLoader.dismissProgressLoader();
                    }
                }

                @Override
                public void onFailure(Call<AuthSignUpRes> call, Throwable t) {
                    System.out.println("Error: " + t.getMessage()); // Print the error message
                    Toast.makeText(SignUp.this, "Server Error", Toast.LENGTH_SHORT).show();
                    progressLoader.dismissProgressLoader();
                }
            });
            count = 0;
        }
        count = 0;
    }

}