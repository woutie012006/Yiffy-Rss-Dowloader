import javax.swing.*;
import java.util.ArrayList;
import java.util.Date;

/**
 * Created by woute on 16-8-2015.
 */
public class Movie {

    private String id;
    private String url;
    private String imdbCode;
    private String title;
    private Integer year;
    private double rating;
    private ArrayList<String> genres;
    private ImageIcon image;
    private ArrayList<Torrent> torrents;
    private Date uploaded;

    public String getDescription()
    {
        return description;
    }

    public void setDescription(String description)
    {
        this.description = description;
    }

    private String description;

    public Movie(String id, String url, String imdbCode, String title, Integer year, double rating, ArrayList<String> genres, ImageIcon image, ArrayList<Torrent> torrents, Date uploaded, String description) {
        this.id = id;
        this.url = url;
        this.imdbCode = imdbCode;
        this.title = title;
        this.year = year;
        this.rating = rating;
        this.genres = genres;
        this.image = image;
        this.torrents = torrents;
        this.uploaded = uploaded;
        this.description = description;
    }
    public Movie(){}
    public String getId() {

        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getUrl() {
        return url;
    }

    public void setUrl(String url) {
        this.url = url;
    }

    public String getImdbCode() {
        return imdbCode;
    }

    public void setImdbCode(String imdbCode) {
        this.imdbCode = imdbCode;
    }

    public String getTitle() {
        return title;
    }

    public void setTitle(String title) {
        this.title = title;
    }

    public Integer getYear() {
        return year;
    }

    public void setYear(Integer year) {
        this.year = year;
    }

    public double getRating() {
        return rating;
    }

    public void setRating(double rating) {
        this.rating = rating;
    }

    public ArrayList<String> getGenres() {
        return genres;
    }

    public void setGenres(ArrayList<String> genres) {
        this.genres = genres;
    }

    public ImageIcon getImage() {
        return image;
    }

    public void setImage(ImageIcon image) {
        this.image = image;
    }

    public ArrayList<Torrent> getTorrents() {
        return torrents;
    }

    public void setTorrents(ArrayList<Torrent> torrents) {
        this.torrents = torrents;
    }

    public Date getUploaded() {
        return uploaded;
    }

    public void setUploaded(Date uploaded) {
        this.uploaded = uploaded;
    }

    @Override
    public String toString()
    {
        return title;
    }
}
