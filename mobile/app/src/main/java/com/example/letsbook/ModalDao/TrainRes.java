package com.example.letsbook.ModalDao;

import java.io.Serializable;
import java.util.List;

public class TrainRes implements Serializable {
    private int page;
    private int perPage;
    private int totalItems;
    private int totalPages;
    private List<TrainItem> items;

    public TrainRes(int page, int perPage, int totalItems, int totalPages, List<TrainItem> items) {
        this.page = page;
        this.perPage = perPage;
        this.totalItems = totalItems;
        this.totalPages = totalPages;
        this.items = items;
    }

    public int getPage() {
        return page;
    }

    public void setPage(int page) {
        this.page = page;
    }

    public int getPerPage() {
        return perPage;
    }

    public void setPerPage(int perPage) {
        this.perPage = perPage;
    }

    public int getTotalItems() {
        return totalItems;
    }

    public void setTotalItems(int totalItems) {
        this.totalItems = totalItems;
    }

    public int getTotalPages() {
        return totalPages;
    }

    public void setTotalPages(int totalPages) {
        this.totalPages = totalPages;
    }

    public List<TrainItem> getItems() {
        return items;
    }

    public void setItems(List<TrainItem> items) {
        this.items = items;
    }
}
