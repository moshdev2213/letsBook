package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;

import android.os.Bundle;
import android.view.View;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.Toast;

import com.example.letsbook.Modal.User;
import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.R;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

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

        if(out!=null){
        etUserEmailEdtPro.setText(out.getItems().get(0).getEmail());
        etUserTelEdtPro.setText(""+out.getItems().get(0).getPhone());
        etUserNameEdtPro.setText(out.getItems().get(0).getName());
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
        FabImgUpload.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Toast.makeText(getApplicationContext(),"UnderDev",Toast.LENGTH_SHORT).show();
            }
        });
    }

}