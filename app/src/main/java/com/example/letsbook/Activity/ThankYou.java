package com.example.letsbook.Activity;

import androidx.appcompat.app.AppCompatActivity;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import com.example.letsbook.Modal.UserRecord;
import com.example.letsbook.R;

public class ThankYou extends AppCompatActivity {
    private UserRecord userRecord;
    private Button cvGoBackToHomeBtn;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_thank_you);
    }
}