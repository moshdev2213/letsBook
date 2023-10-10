package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.cardview.widget.CardView;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.R;

public class ThankYou extends AppCompatActivity {
    private UserRecord receivedUser;
    private CardView cvGoBackToHomeBtn;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thank_you);
        receivedUser = (UserRecord) getIntent().getSerializableExtra("user");

        cvGoBackToHomeBtn =findViewById(R.id.cvGoBackToHomeBtn);
        cvGoBackToHomeBtn.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                Intent intent = new Intent(ThankYou.this, Home.class);
                intent.putExtra("user", receivedUser); // Assuming "user" is Parcelable or Serializable
                startActivity(intent);
                finish();
            }
        });
    }
}