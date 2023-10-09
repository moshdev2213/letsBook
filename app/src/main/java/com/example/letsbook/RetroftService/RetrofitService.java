package com.example.letsbook.RetroftService;

import com.google.gson.Gson;

import java.util.concurrent.TimeUnit;

import okhttp3.OkHttpClient;
import retrofit2.Retrofit;
import retrofit2.converter.gson.GsonConverterFactory;

public class RetrofitService {
    private Retrofit retrofit;

    public RetrofitService() {
        this.retrofit = initRetrofit();
    }

    private Retrofit initRetrofit() {
        // Create a custom OkHttpClient with extended timeout settings
        OkHttpClient okHttpClient = new OkHttpClient.Builder()
                .connectTimeout(30, TimeUnit.SECONDS) // Set the connection timeout to 30 seconds
                .readTimeout(30, TimeUnit.SECONDS)    // Set the read timeout to 30 seconds
                .writeTimeout(30, TimeUnit.SECONDS)   // Set the write timeout to 30 seconds
                .build();

        return new Retrofit.Builder()
                .baseUrl("https://lets-book.pockethost.io")
                .addConverterFactory(GsonConverterFactory.create(new Gson()))
                .client(okHttpClient)
                .build();
    }

    public Retrofit getRetrofit() {
        return this.retrofit;
    }
}