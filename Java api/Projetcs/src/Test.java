import com.sun.xml.internal.ws.api.streaming.XMLStreamReaderFactory;
import org.json.simple.JSONArray;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;
import org.json.simple.parser.ParseException;

import javax.imageio.ImageIO;
import javax.swing.*;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;
import java.awt.*;
import java.io.*;
import java.net.URL;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.Locale;


/**
 * Created by woute on 16-8-2015.
 */
public class Test extends JFrame
{
    private JPanel panel1;
    private JLabel imgLbl;
    private ArrayList<Movie> movs = new ArrayList<>();
    private JList list1;
    private JScrollPane scrollPane;
    private JTextArea textArea1;


    public static void main(String[] args)
    {
        JFrame frame = new JFrame();
        frame.setContentPane(new Test().panel1);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.pack();
        frame.setVisible(true);
        frame.setSize(600, 380);
        Dimension d = Toolkit.getDefaultToolkit().getScreenSize();
        frame.setLocation((int) d.getWidth() / 2 - +(frame.getWidth() / 2), (int) d.getHeight() / 2 - (frame.getHeight() / 2));
    }

    public Test()
    {
        list1.setSize(400, 900);
        scrollPane.setSize(300,380);
        String Url = "https://yts.to/api/v2/list_movies.json";
        String code = "";
        code = getPageSource(Url);

        JSONstuff(code);

        //lbl1.setSize(img.getIconWidth(),img.getIconHeight());

       // list1.addListSelectionListener(new ListSelectionListener() {});
        list1.addListSelectionListener(new ListSelectionListener()
        {
            @Override
            public void valueChanged(ListSelectionEvent e)
            {
                imgLbl.setIcon(((Movie) list1.getSelectedValue()).getImage());
                textArea1.setText(((Movie) list1.getSelectedValue()).getDescription());
            }
        });
    }

    public String getPageSource(String Url)
    {
        String output = "";
        try {
            URL site = new URL(Url);
            BufferedReader in = new BufferedReader(
                    new InputStreamReader(
                            site.openStream()));

            String inputLine;

            while ((inputLine = in.readLine()) != null)
                output += inputLine;

            in.close();
        } catch (Exception e) {
        }
        System.out.println(output);
        return output;
    }

    public static ImageIcon getPic(String url)
    {
        Image image = null;
        try {
            URL Url = new URL(url);
            image = ImageIO.read(Url);
        } catch (IOException e) {
        }
        return new ImageIcon(image);
    }



    public void JSONstuff(String code)
    {
        JSONParser parser = new JSONParser();
        JSONParser descrParser = new JSONParser();
        //try{
        Object obj = null;
        try {
            obj = parser.parse(code);
        } catch (Exception e) {
        }
        JSONObject obj2 = (JSONObject) obj;
        JSONObject obj3 = (JSONObject) obj2.get("data");
        JSONArray movies = (JSONArray) obj3.get("movies");

        for (int i = 0; i < movies.size(); i++) {

            JSONObject movie = (JSONObject) movies.get(i);
            String k = movie.get("title").toString();

            ImageIcon img = getPic(movie.get("medium_cover_image").toString());
            if (i == 0) {
                imgLbl.setIcon(img);
            }
            String id = (String) movie.get("id").toString();
            String url = (String) movie.get("url".toString());
            String imdbCode = (String) movie.get("imdb_code".toString());
            String title = (String) movie.get("title".toString());

            Integer year = (int) ((long) movie.get("year"));
            Double rating = Double.parseDouble(movie.get("rating").toString());
            //ArrayList<String> genres = (String)movie.get("url".toString());
            ImageIcon image = getPic((String) movie.get("medium_cover_image".toString()));
            //ArrayList<String> torrents = torrents;
            DateFormat format = new SimpleDateFormat("MMMM d, yyyy", Locale.ENGLISH);
            Date uploaded = null;
            try            {
                uploaded = format.parse(movie.get("title").toString());
            } catch (Exception e) {
            }

            //get the description
            String descCode = getPageSource("https://yts.to/api/v2/movie_details.json?movie_id=" + id);
            JSONObject obj4=null;
            try {
                obj4 =  (JSONObject)descrParser.parse(descCode);
            } catch (ParseException e) {}
            JSONObject obj5 = (JSONObject) obj4.get("data");
            String description =(String) obj5.get("description_full");
            movs.add(new Movie(id, url, imdbCode, title, year, rating, null, image, null, uploaded, description));


        }
        list1.setListData(movs.toArray());
        textArea1.setText(movs.get(0).getDescription());

    }


}
