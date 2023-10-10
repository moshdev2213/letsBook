package com.example.letsbook.DialogAlerts;

import android.app.Dialog;
import android.content.Context;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.view.Gravity;
import android.view.ViewGroup;
import android.view.Window;
import android.widget.Button;
import android.widget.TextView;

import com.example.letsbook.R;

public class OkNoDialog {
    private Context context;
    private Dialog dialog;
    private TextView tvDescription;
    private Button btnDialogSuccess;

    public OkNoDialog(Context context) {
        this.context = context;
        this.dialog = new Dialog(context);
    }

    public void dialogWithSuccess(String description, Runnable onDismiss) {
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setCancelable(false);
        dialog.setContentView(R.layout.success_card);
        dialog.getWindow().setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT);
        dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        dialog.getWindow().setGravity(Gravity.BOTTOM);
        tvDescription = dialog.findViewById(R.id.tvDescription);
        btnDialogSuccess = dialog.findViewById(R.id.btnDialogSuccess);

        tvDescription.setText(description);
        btnDialogSuccess.setOnClickListener(v -> {
            onDismiss.run();
            dialog.dismiss();
        });
        dialog.show();
    }
}
