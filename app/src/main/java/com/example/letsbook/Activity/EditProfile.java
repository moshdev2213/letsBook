package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;

import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.Toast;

import com.example.letsbook.ApiRoutes.EditUserApi;
import com.example.letsbook.DialogAlerts.OkNoDialog;
import com.example.letsbook.DialogAlerts.ProgressLoader;
import com.example.letsbook.Modal.User;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.ModalDao.UpdatedUser;
import com.example.letsbook.ModalDao.UserItem;
import com.example.letsbook.R;
import com.example.letsbook.RetroftService.RetrofitService;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import retrofit2.Call;
import retrofit2.Callback;
import retrofit2.Response;

public class EditProfile extends AppCompatActivity {
    private ImageView imbBackBtn;
    private FloatingActionButton FabImgUpload;
    private CardView cvEditBackBtn;
    private CardView cvEditProBtn;
    private EditText etUserNameEdtPro;
    private EditText etUserNewPwdEdtPro;
    private EditText etUserOldPwdEdtPro;
    private EditText etUserTelEdtPro;
    private EditText etUserEmailEdtPro;
    private OkNoDialog okNoDialog;
    private String token;
    private User out;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_edit_profile);

        imbBackBtn = findViewById(R.id.imbBackBtn);
        FabImgUpload = findViewById(R.id.FabImgUpload);
        cvEditBackBtn = findViewById(R.id.cvEditBackBtn);
        cvEditProBtn = findViewById(R.id.cvEditProBtn);
        etUserNameEdtPro = findViewById(R.id.etUserNameEdtPro);
        etUserNewPwdEdtPro = findViewById(R.id.etUserNewPwdEdtPro);
        etUserOldPwdEdtPro = findViewById(R.id.etUserOldPwdEdtPro);
        etUserTelEdtPro = findViewById(R.id.etUserTelEdtPro);
        etUserEmailEdtPro = findViewById(R.id.etUserEmailEdtPro);

        out = (User) getIntent().getSerializableExtra("user");
        token = getIntent().getStringExtra("token");

        if(out!=null){
        etUserEmailEdtPro.setText(out.getItems().get(0).getEmail());
//        etUserTelEdtPro.setText(""+out.getItems().get(0).getPhone());
        etUserNameEdtPro.setText(out.getItems().get(0).getName());
//        Toast.makeText(this,""+out.getItems().get(0).getId(),Toast.LENGTH_SHORT).show();
//        Toast.makeText(this,""+token,Toast.LENGTH_SHORT).show();
        }

        imbBackBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });
        cvEditBackBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                finish();
            }
        });
        cvEditProBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                editUserProfile(token);
            }
        });
        FabImgUpload.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Toast.makeText(getApplicationContext(),"UnderDev",Toast.LENGTH_SHORT).show();
            }
        });
    }
    private void editUserProfile(String tokens){

        RetrofitService retrofitService = new RetrofitService();
        EditUserApi authService = retrofitService.getRetrofit().create(EditUserApi.class);

        Call<UpdatedUser> call = authService.updateUserDetails(
                tokens,
                out.getItems().get(0).getId(),
                new UserItem(
                        out.getItems().get(0).getEmail(),
                        out.getItems().get(0).getId(),
                        etUserNameEdtPro.getText().toString(),
                        Long.parseLong(etUserTelEdtPro.getText().toString()),
                        out.getItems().get(0).getNic(),
                        out.getItems().get(0).getVerified()
                )
        );
        call.enqueue(new Callback<UpdatedUser>() {
            @Override
            public void onResponse(Call<UpdatedUser> call, Response<UpdatedUser> response) {
                if (response.isSuccessful()) {
                    UpdatedUser user = response.body();
                    if (user != null) {
                        Toast.makeText(getApplicationContext(), "Done editing", Toast.LENGTH_SHORT).show();
                        finish();
                    }
                } else {
                    Toast.makeText(getApplicationContext(), "Invalid Credentials", Toast.LENGTH_SHORT).show();
                    finish();
                }
            }

            @Override
            public void onFailure(Call<UpdatedUser> call, Throwable t) {
                System.out.println("Error: " + t.getMessage()); // Print the error message
                Toast.makeText(getApplicationContext(), "Server Error", Toast.LENGTH_SHORT).show();
                finish();
            }
        });
    }

}