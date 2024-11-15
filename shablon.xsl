<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <!-- Шаблон для початку перетворення -->
  <xsl:template match="/">

    <html lang="uk">
      <head>
        <meta charset="UTF-8"/>
        <title>Інформація про ресурси кафедри</title>
        <style>
          body {
            font-family: Arial, sans-serif;
            background-color: #f5f5f5;
            color: #333;
            margin: 20px;
            padding: 0;
          }
          h1 {
            color: #007ACC;
            text-align: center;
          }
          table {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
          }
          table, th, td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
          }
          th {
            background-color: #007ACC;
            color: white;
          }
          tr:nth-child(even) {
            background-color: #f9f9f9;
          }
        </style>
      </head>
      <body>
        <h1>"Наукове товариство студентів та аспірантів"</h1>
        
        <!-- Таблиця -->
        <table>
          <thead>
            <tr>
              <th>П.І.П.</th>
              <th>Факультет</th>
              <th>Кафедра</th>
              <th>Спеціальність</th>
              <th>Час вступу</th>
              <th>План заходів</th>
            </tr>
          </thead>
          <tbody>
            <!-- Ітерація по всіх ресурсах -->
            <xsl:for-each select="resources/resource">
              <tr>
                <td><xsl:value-of select="@full_name"/></td>
                <td><xsl:value-of select="@faculty"/></td>
                <td><xsl:value-of select="@department"/></td>
                <td><xsl:value-of select="@specialty"/></td>
                <td><xsl:value-of select="@joining_time"/></td>
                <td>
                  <!-- Ітерація по плану заходів -->
                  <xsl:for-each select="event_plan/event">
                    <div>
                      <b>Опис:</b> <xsl:value-of select="@description"/><br/>
                      <b>Локація:</b> <xsl:value-of select="@location"/><br/>
                      <b>Час:</b> <xsl:value-of select="@time"/><br/>
                      <b>Неформальна характеристика:</b> <xsl:value-of select="@informal_characteristic"/><br/>
                    </div>
                    <hr/>
                  </xsl:for-each>
                </td>
              </tr>
            </xsl:for-each>
          </tbody>
        </table>
      </body>
    </html>

  </xsl:template>

</xsl:stylesheet>
