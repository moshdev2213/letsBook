package com.example.letsbook.DialogAlerts;

import android.app.Dialog;
import android.content.Context;
import android.graphics.Color;
import android.graphics.drawable.ColorDrawable;
import android.view.Window;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.example.letsbook.R;

public class ProgressLoader {
    private Context context;
    private String title;
    private String description;
    private Dialog dialog;

    public ProgressLoader(Context context, String title, String description) {
        this.context = context;
        this.title = title;
        this.description = description;
        this.dialog = new Dialog(context);
    }

    public void startProgressLoader() {
        dialog.requestWindowFeature(Window.FEATURE_NO_TITLE);
        dialog.setCancelable(false);
        dialog.setContentView(R.layout.progress_loader);
        dialog.getWindow().setBackgroundDrawable(new ColorDrawable(Color.TRANSPARENT));
        dialog.setCancelable(false);

        TextView tvDescritionLoader = dialog.findViewById(R.id.tvDescritionLoader);
        TextView tvTitleLoader = dialog.findViewById(R.id.tvTitleLoader);
        ProgressBar pbLoader = dialog.findViewById(R.id.pbLoader);

        tvDescritionLoader.setText(description);
        tvTitleLoader.setText(title);

        dialog.show();
    }

    public void dismissProgressLoader() {
        dialog.dismiss();
    }
}
